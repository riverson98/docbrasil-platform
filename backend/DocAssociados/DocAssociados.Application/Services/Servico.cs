using AutoMapper;
using DocAssociados.Application.Config;
using DocAssociados.Application.Extensions;
using DocAssociados.Application.Interfaces;
using DocAssociados.Domain.Entities;
using DocAssociados.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DocAssociados.Application.Services;

public class Servico<TDto, TEntidade> : IServico<TDto, TEntidade>
                                      where TDto : class
                                      where TEntidade : class
{

    protected readonly IUnityOfWork _unityOfWork;
    protected readonly IMapper _mapper;
    protected readonly IRepositorio<TEntidade> _repositorio;
    protected readonly IServicoAzure<TDto> _servicoAzure;

    public Servico(IUnityOfWork unityOfWork, IMapper mapper, IRepositorio<TEntidade> repositorio, IServicoAzure<TDto> servicoAzure)
    {
        _unityOfWork = unityOfWork;
        _mapper = mapper;
        _repositorio = repositorio;
        _servicoAzure = servicoAzure;
    }

    public async Task<TDto?> BuscaAsync(Expression<Func<TDto, bool>> predicate)
    {
        Expression<Func<TEntidade, bool>> predicatoDaEntidade = predicate.ConvertExpression<TDto, TEntidade>();
        var entidade = await _repositorio.BuscaAsync(predicatoDaEntidade);

        return _mapper.Map<TDto>(entidade);
    }

    public async Task<IEnumerable<TDto>> BuscaTodosAsync()
    {
        var entidades = await _repositorio.BuscaTodosAsync();
        return _mapper.Map<IEnumerable<TDto>>(entidades);
    }

    public async Task<IEnumerable<TDto>> BuscaTodosAsync(Expression<Func<TDto, bool>> predicate)
    {
        Expression<Func<TEntidade, bool>> predicatoDaEntidade = predicate.ConvertExpression<TDto, TEntidade>();
        var entidadesEncontradas = await _repositorio.BuscaTodosAsync(predicatoDaEntidade);

        return _mapper.Map<IEnumerable<TDto>>(entidadesEncontradas);
    }

    public async Task<ResultadoPaginado<TDto>> BuscaDtoComPaginacaoAsync(ParametrosDaPaginacao parametros, 
        Expression<Func<TDto, bool>>? predicato = null, Expression<Func<TDto, bool>>? filtro = null)
    {
        Expression<Func<TEntidade, bool>>? predicatoDaEntidade = null;
        Expression<Func<TEntidade, bool>>? filtroDaEntidade = null;

        if (predicato != null)
            predicatoDaEntidade = predicato.ConvertExpression<TDto, TEntidade>();
        
        if (filtro != null)
            filtroDaEntidade = filtro.ConvertExpression<TDto, TEntidade>();

        var resultadoPaginadoDaEntidade = await _repositorio.BuscaEntidadeComPaginacaoAsync(parametros.Pagina,
                                                        parametros.QuantidadeDeItensPorPagina, predicatoDaEntidade, filtroDaEntidade);

        return _mapper.Map<ResultadoPaginado<TDto>>(resultadoPaginadoDaEntidade);
    }

    public async Task<TDto> AdicionaAsync(TDto dto)
    {
        var entidade = _mapper.Map<TEntidade>(dto);
        var entidadeAdicionada = _repositorio.Adiciona(entidade);

        await _unityOfWork.CommitAsync();

        return _mapper.Map<TDto>(entidadeAdicionada);
    }

    public async Task<IEnumerable<TDto>> AdicionaTodosAsync(IEnumerable<TDto> dtos)
    {
        var entidades = _mapper.Map<IEnumerable<TEntidade>>(dtos);
        var entidadesAdicionada = await _repositorio.AdicionaTodos(entidades);

        await _unityOfWork.CommitAsync();

        return _mapper.Map<IEnumerable<TDto>>(entidadesAdicionada);
    }

    public async Task<TDto> AtualizaAsync(TDto dto)
    {
        var entidade = _mapper.Map<TEntidade>(dto);
        var entidadeAtualizada = _repositorio.Atualiza(entidade);

        await _unityOfWork.CommitAsync();

        return _mapper.Map<TDto>(entidadeAtualizada);
    }

    public async Task DeletaAsync(TDto dto)
    {
        var entidade = _mapper.Map<TEntidade>(dto);

        _repositorio.Deleta(entidade);
        await _unityOfWork.CommitAsync();

    }

    public async Task UploadImagemAsync(IEnumerable<IUploadable> dtos, string? email)
    {
        await _servicoAzure.UploadImagem(dtos, email);
    }

    public async Task<string> AtualizaTokenSas(string urlTokenSas)
    {
        return await _servicoAzure.AtualizaTokenSas(urlTokenSas);
    }

    public async Task DeleteBlobItensAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;

        await _servicoAzure.DeleteBlobAsync(url);
    }
}
