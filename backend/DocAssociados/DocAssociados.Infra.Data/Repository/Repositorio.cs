using DocAssociados.Domain.Entities;
using DocAssociados.Domain.Interfaces;
using DocAssociados.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DocAssociados.Infra.Data.Repository;

public class Repositorio<T> : IRepositorio<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repositorio(AppDbContext context)
    {
        _context = context;
    }

    public T Adiciona(T entity)
    {
        _context.Add(entity);
        return entity;
    }

    public async Task<IEnumerable<T>> AdicionaTodos(IEnumerable<T> entidades)
    {
        if (entidades == null || !entidades.Any())
            throw new ArgumentException("As entidades não podem está nula ou vazia");

        await _context.AddRangeAsync(entidades);

        return entidades;
    }

    public T Atualiza(T entity)
    {
        _context.Set<T>().Update(entity);
        return entity;
    }

    public async Task<T?> BuscaAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate);
    }

    public async Task<ResultadoPaginado<T>> BuscaEntidadeComPaginacaoAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var query = _context.Set<T>().AsNoTracking().AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        var totalDeItens = await query.CountAsync();
        var itens = await query.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return new ResultadoPaginado<T>
        {
            Itens = itens,
            TotalDeItens = totalDeItens,
            Pagina = page,
            QuantidadeDeItensPorPagina = pageSize
        };
    }

    public async Task<IEnumerable<T>> BuscaTodosAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<T>> BuscaTodosAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
    }

    public T Deleta(T entity)
    {
        _context.Set<T>().Remove(entity);
        return entity;
    }
}
