using DocAssociados.Domain.Entities;
using DocAssociados.Service.Domain.EntitiesSummary;
using System.Linq.Expressions;

namespace DocAssociados.Domain.Interfaces;

public interface IAssociadoRepositorio : IRepositorio<Associado>
{
    Associado AdicionaAssociadoComEndereco(Associado associado);
    Task<AssociadoResumido> AtualizaParcialmenteAsync(AssociadoResumido entidadeResumida);
    Task<Associado> BuscaAssociadoComEndereco(Expression<Func<Associado, bool>> predicate);
    Task<AssociadoResumido> BuscaAssociadoResumidoAsync(Guid id);
}
