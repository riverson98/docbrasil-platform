using DocAssociados.Domain.Entities;
using DocAssociados.Domain.Interfaces;
using DocAssociados.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DocAssociados.Infra.Data.Repository;

public class AssociadoRepositorio : Repositorio<Associado>, IAssociadoRepositorio
{
    public AssociadoRepositorio(AppDbContext context) : base(context)
    {
    }

    public Associado AdicionaAssociadoComEndereco(Associado associado)
    {
        if (associado == null || associado.Endereco == null)
            throw new ArgumentNullException("Associado ou Endereço não podem ser nulos.");

        _context.Associado.Add(associado);

        associado.Endereco.AssociadoId = associado.Id;

        _context.Endereco.Add(associado.Endereco);

        return associado;
    }

    public async Task<Associado> BuscaAssociadoComEndereco(Expression<Func<Associado, bool>> predicate)
    {
        return await _context.Set<Associado>()
                             .Include(associado => associado.Endereco)
                             .AsNoTracking()
                             .FirstOrDefaultAsync(predicate);
    }
}
