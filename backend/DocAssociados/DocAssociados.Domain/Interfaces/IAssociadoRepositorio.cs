using DocAssociados.Domain.Entities;
using System.Linq.Expressions;

namespace DocAssociados.Domain.Interfaces;

public interface IAssociadoRepositorio : IRepositorio<Associado>
{
    Associado AdicionaAssociadoComEndereco(Associado associado);
    Task<Associado> BuscaAssociadoComEndereco(Expression<Func<Associado, bool>> predicate);
}
