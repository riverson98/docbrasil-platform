using DocAssociados.Application.Config;
using DocAssociados.Domain.Entities;
using System.Linq.Expressions;

namespace DocAssociados.Application.Interfaces;

public interface IServico<TDto, TEntidade>
{
    Task<IEnumerable<TDto>> BuscaTodosAsync();
    Task<IEnumerable<TDto>> BuscaTodosAsync(Expression<Func<TDto, bool>> predicate);
    Task<TDto?> BuscaAsync(Expression<Func<TDto, bool>> predicate);
    Task<TDto> AdicionaAsync(TDto dto);
    Task<IEnumerable<TDto>> AdicionaTodosAsync(IEnumerable<TDto> dtos);
    Task<TDto> AtualizaAsync(TDto dto);
    Task DeletaAsync(TDto dto);
    Task UploadImagemAsync(IEnumerable<IUploadable> dtos, string? email);
    Task<ResultadoPaginado<TDto>> BuscaDtoComPaginacaoAsync(ParametrosDaPaginacao parametros,
        Expression<Func<TDto, bool>>? predicato = null);
}
