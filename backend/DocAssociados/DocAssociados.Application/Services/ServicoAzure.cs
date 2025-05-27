using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using DocAssociados.Application.Interfaces;
using DocAssociados.Service.Infra.CrossCutting.Config;
using DocAssociados.Service.Infra.CrossCutting.AzureIdentity;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using Azure.Identity;

namespace DocAssociados.Application.Services;

public class ServicoAzure<T> : IServicoAzure<T> where T : IUploadable
{
    private readonly AzureBlobStorageOpcoes _opcoes;
    private readonly IKeyVaultService _keyService;
    private readonly IMemoryCache _cache;
    private readonly BlobContainerClient _containerClient;

    public ServicoAzure(AzureBlobStorageOpcoes opcoes, IKeyVaultService keyService, IMemoryCache cache)
    {
        _opcoes = opcoes;
        _keyService = keyService;
        _cache = cache;

        var blobServiceClient = new BlobServiceClient(new Uri(_opcoes.ServicoBlobUrl));
        _containerClient = blobServiceClient.GetBlobContainerClient(_opcoes.NomeDoContainer);
    }

    public async Task UploadImagem(IEnumerable<IUploadable> dtos, string? email)
    {
        try
        {
            if (dtos.Any(dto => dto == null || dto.FotoDoArquivo?.Length == 0))
            {
                throw new ArgumentException("Os arquivos são obrigatórios");
            }

            var tasks = dtos.Where(dto => dto.FotoDoArquivo != null && dto.FotoDoArquivo.Length > 0)
                .Select(async dto =>
                {
                    var nomeDoArquivo = $"user-{email}/{dto.SufixoBlob}";
                    
                    
                    using (var stream = dto.FotoDoArquivo.OpenReadStream())
                    {
                        var sasToken = await CriaSasToken(nomeDoArquivo);
                        var baseUrl = $"{_opcoes.ServicoBlobUrl}/{_opcoes.NomeDoContainer}/{nomeDoArquivo}?{sasToken}";
                        var blobClient = new BlobClient(new Uri(baseUrl), new DefaultAzureCredential());

                        await blobClient.UploadAsync(stream, true);

                        var uri = blobClient.Uri.ToString();

                        if (!string.IsNullOrEmpty(uri))
                            dto.UrlDaFoto = uri;

                        return blobClient.Uri.ToString();
                    }
                });

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task<string> UploadImagem(T dto, string? email)
    {
        if (!dto.TemArquivoParaUpload)
            throw new ArgumentException("O arquivo é obrigatório");

        var nomeDoArquivo = $"user-{email}/{dto.SufixoBlob}";


        using (var stream = dto.FotoDoArquivo.OpenReadStream())
        {
            BlobClient blobClient;
            string baseUrl;

            if (_opcoes.UsaSasToken)
            {
                var sasToken = await CriaSasToken(nomeDoArquivo);
                baseUrl = $"{_opcoes.ServicoBlobUrl}/{_opcoes.NomeDoContainer}/{nomeDoArquivo}?{sasToken}";
            }
            else
                baseUrl = $"{_opcoes.ServicoBlobUrl}/{_opcoes.NomeDoContainer}/{nomeDoArquivo}";

            blobClient = new BlobClient(new Uri(baseUrl));

            await blobClient.UploadAsync(stream, true);

            var uri = blobClient.Uri.ToString();

            if (!string.IsNullOrEmpty(uri))
                dto.UrlDaFoto = uri;

            return blobClient.Uri.ToString();
        }
    }

    public async Task<string> RecriaImagemUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return string.Empty;

        var uri = new Uri(url);
        var nomeDoArquivo = uri.Segments.Last();
        nomeDoArquivo = $"userId-{nomeDoArquivo.Split("_")[0]}/{nomeDoArquivo}";
        var novoSasToken = await CriaSasToken(nomeDoArquivo);

        return $"{_opcoes.ServicoBlobUrl}/{_opcoes.NomeDoContainer}/{nomeDoArquivo}?{novoSasToken}";
    }

    public bool TokenEstaExpirado(string sasToken)
    {
        var queryParams = HttpUtility.ParseQueryString(sasToken);
        var expiracaoString = queryParams["se"];

        if (DateTime.TryParseExact(
            expiracaoString,
            "yyyy-MM-ddTHH:mm:ssZ",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
            out DateTime tempoDeExpiracao))
        {
            return tempoDeExpiracao <= DateTime.UtcNow;
        }

        return true;
    }

    public async Task<string> AtualizaTokenSas(string urlTokenSas)
    {
        if (string.IsNullOrEmpty(urlTokenSas))
            return string.Empty;

        if (!TokenEstaExpirado(urlTokenSas))
            return urlTokenSas;

        var nomeDoArquivo = ExtraiNomeDoArquivoDaUrl(urlTokenSas);
        var tokenAtualizado = await CriaSasToken(nomeDoArquivo);

        return $"{_opcoes.ServicoBlobUrl}/{_opcoes.NomeDoContainer}/{nomeDoArquivo}?{tokenAtualizado}";
    }

    public async Task DeleteBlobAsync(string url)
    {
        var nomeDoArquivo = ExtraiNomeDoArquivoDaUrl(url);
        var sasToken = await CriaSasToken(nomeDoArquivo);
        var baseUrl = $"{_opcoes.ServicoBlobUrl}/{_opcoes.NomeDoContainer}/{nomeDoArquivo}?{sasToken}";
        var blobClient = new BlobClient(new Uri(baseUrl));
        await blobClient.DeleteIfExistsAsync();
    }

    private async Task<string> CriaSasToken(string nomeDoArquivo)
    {

        var chaveDoSasTokenNoCache = $"SasToken-cache_{nomeDoArquivo}";
        var sasTokenCache = ObtemSasTokenViaCache(chaveDoSasTokenNoCache);


        if (!string.IsNullOrEmpty(sasTokenCache))
            return sasTokenCache;


        var blobContainerClient = new BlobContainerClient(new Uri($"{_opcoes.ServicoBlobUrl}/{_opcoes.NomeDoContainer}"));
        var blobClient = blobContainerClient.GetBlobClient(nomeDoArquivo);


        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = blobContainerClient.Name,
            BlobName = blobClient.Name,
            Resource = "b",
            StartsOn = DateTime.UtcNow.AddMinutes(-5),
            ExpiresOn = DateTime.UtcNow.AddMinutes(5)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Delete);
        var blobSecretKey = await _keyService.GetSecretAsync("AzureBlobContainerSecretKey");
        var storageSharedKeyCredential = new StorageSharedKeyCredential(_opcoes.NomeDaConta, blobSecretKey);

        var sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();
        
        var expiracao = DateTime.UtcNow.AddMinutes(4);
        _cache.Set(chaveDoSasTokenNoCache, sasToken, expiracao);

        return sasToken;
    }

    private string ObtemSasTokenViaCache(string chaveDoSasTokenNoCache)
    {

        if (_cache.TryGetValue(chaveDoSasTokenNoCache, out string sasToken))
            if (!TokenEstaExpirado(sasToken))
                return sasToken;

        return string.Empty;
    }

    private string ExtraiNomeDoArquivoDaUrl(string sasUrl)
    {
        var uri = new Uri(sasUrl);
        var containerName = _opcoes.NomeDoContainer;

        var absolutePath = uri.AbsolutePath;
        var prefixo = $"/{containerName}/";

        if (absolutePath.StartsWith(prefixo))
        {
            return absolutePath.Substring(prefixo.Length);
        }

        throw new InvalidOperationException("URL SAS inválida ou container não corresponde.");
    }
}
