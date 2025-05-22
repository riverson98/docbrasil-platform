using AutoMapper;
using DocAssociados.Application.DTOs;
using DocAssociados.Application.Extensions;
using DocAssociados.Application.Interfaces;
using DocAssociados.Domain.Entities;
using DocAssociados.Domain.Interfaces;
using DocAssociados.Service.Application.DTOs;
using DocAssociados.Service.Application.Enums;
using DocAssociados.Service.Domain.EntitiesSummary;
using DocAssociados.Service.Infra.CrossCutting.Config;
using DocAssociados.Service.Infra.CrossCutting.HttpClients.Interfaces;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace DocAssociados.Application.Services;

public class ServicoAssociado : Servico<AssociadoDto, Associado>, IServicoAssociado
{
    private readonly IAssociadoRepositorio _associadoRepositorio;
    private readonly IHttpClientDefault<AssociadoDto> _httpClient;
    private readonly ApiGatewayConfig _apiGatewayConfig;
    public ServicoAssociado(IUnityOfWork unityOfWork, IMapper mapper, IRepositorio<Associado> repositorio,
        IServicoAzure<AssociadoDto> servicoAzure, IAssociadoRepositorio associadoRepositorio,
        IHttpClientDefault<AssociadoDto> httpClient, ApiGatewayConfig apiGatewayConfig) : base(unityOfWork, mapper, repositorio, servicoAzure)
    {
        _associadoRepositorio = associadoRepositorio;
        _httpClient = httpClient;
        _apiGatewayConfig = apiGatewayConfig;
    }

    public async Task<AssociadoDto> AdicionaAssociadoComEnderecoAsync(AssociadoDto associadoDto)
    {
        var EntidadeAssociado = _mapper.Map<Associado>(associadoDto);

        var associadoAdicionado = _associadoRepositorio.AdicionaAssociadoComEndereco(EntidadeAssociado);

        await _unityOfWork.CommitAsync();

        if (associadoDto.Funcao.Equals(Funcoes.ADMINISTRADOR))
        {
            var authApiUrl = $"http://{_apiGatewayConfig.Url}/auth/register";
            var cpf = Regex.Replace(associadoAdicionado.Cpf, @"\D", "");
            var body = new AutenticacaoDto
            {
                Id = associadoAdicionado.Id,
                Name = associadoAdicionado.Nome,
                Email = associadoAdicionado.Email,
                Password = $"{associadoAdicionado.Nome.Substring(0, 1).ToUpper()}{cpf}@doc"
            };

            await _httpClient.PostDefaultAsync(authApiUrl, body);

        }

        return _mapper.Map<AssociadoDto>(associadoAdicionado);
    }

    public async Task<AssociadoResumidoDto> AtualizaAssociadoParcialmenteAsync(AssociadoResumidoDto associadoResumidoDto)
    {
        var entidadeResumida = _mapper.Map<AssociadoResumido>(associadoResumidoDto);
        var associadoAtualizado = await _associadoRepositorio.AtualizaParcialmenteAsync(entidadeResumida);

        await _unityOfWork.CommitAsync();

        return _mapper.Map<AssociadoResumidoDto>(associadoAtualizado);
    }

    public async Task<AssociadoDto> BuscaAssociadoComEnderecoAsync(Expression<Func<AssociadoDto, bool>> predicate)
    {
        Expression<Func<Associado, bool>> predicatoDaEntidade = predicate.ConvertExpression<AssociadoDto, Associado>();

        var associadoEncontrado = await _associadoRepositorio.BuscaAssociadoComEndereco(predicatoDaEntidade);

        return _mapper.Map<AssociadoDto>(associadoEncontrado);
    }

    public async Task<AssociadoResumidoDto> BuscaAssociadoResumidamenteAsync(Guid id) 
    {
        var associadoEncontrado = await _associadoRepositorio.BuscaAssociadoResumidoAsync(id);
        return _mapper.Map<AssociadoResumidoDto>(associadoEncontrado);
    }

    public async Task DeletaAssociadoAsync(AssociadoDto associadoDto)
    {
        var urlsParaDeletar = new[]
        {
            associadoDto?.CpfUploadUrl,
            associadoDto?.EnderecoDto?.ComprovanteDeResidenciaUpload,
            associadoDto?.TermoAdesaoDto?.TermoAdesaoUploadUrl,
            associadoDto?.FichaAssociadoDto?.FichaAssociacaoUploadUrl,
            associadoDto?.FotoDePerfil?.UrlDaFoto
        }
        .Where(url => !string.IsNullOrWhiteSpace(url))
        .ToList();

        await Task.WhenAll(urlsParaDeletar.Select(url => _servicoAzure.DeleteBlobAsync(url!)));

        await DeletaAsync(associadoDto!);
    }

    public async Task DeletaAssociadoIdentity(AssociadoDto associado)
    {
        var funcoesQuePertencemAoIdentity = new[] { Funcoes.ADMINISTRADOR, Funcoes.DIRETOR };

        if (!funcoesQuePertencemAoIdentity.Contains(associado.Funcao))
            return;

        var authApiUrl = $"http://{_apiGatewayConfig.Url}/auth/delete-user-auth";

        await _httpClient.DeleteDefaultAsync(authApiUrl, associado.Id);
    }
}
