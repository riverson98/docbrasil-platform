namespace DocAssociados.Application.Interfaces;

public interface IServicoAzure<T>
{
    Task UploadImagem(IEnumerable<IUploadable> dtos, string? email);
    Task<string> RecriaImagemUrl(string url);
    bool TokenEstaExpirado(string pathUrl);
    Task<string> AtualizaTokenSas(string urlTokenSas);
    Task DeleteBlobAsync(string url);
}
