using DocAssociados.Domain.Entities;
using System.Linq.Expressions;

namespace DocAssociados.Domain.Interfaces;

public interface IRepositorio<T> where T : class
{
    Task<IEnumerable<T>> BuscaTodosAsync();
    Task<IEnumerable<T>> BuscaTodosAsync(Expression<Func<T, bool>> predicate);
    Task<T?> BuscaAsync(Expression<Func<T, bool>> predicate);
    T Adiciona(T entity);
    T Atualiza(T entity);
    T Deleta(T entity);
    Task<IEnumerable<T>> AdicionaTodos(IEnumerable<T> entities);
    Task<ResultadoPaginado<T>> BuscaEntidadeComPaginacaoAsync(int page, int pageSize,
        Expression<Func<T, bool>>? filtroDinamico = null, Expression<Func<T, bool>>? predicate = null);
}
