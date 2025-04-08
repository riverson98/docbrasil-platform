using DocAssociados.Application.DTOs;
using DocAssociados.Domain.Entities;
using System.Linq.Expressions;

namespace DocAssociados.Application.Interfaces;

public interface IServicoAssociado: IServico<AssociadoDto, Associado>
{
    Task<AssociadoDto> AdicionaAssociadoComEnderecoAsync(AssociadoDto associadoDto);
    Task<AssociadoDto> BuscaAssociadoComEnderecoAsync(Expression<Func<AssociadoDto, bool>> predicate);
}
