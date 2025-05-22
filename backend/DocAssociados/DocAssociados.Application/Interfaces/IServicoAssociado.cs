using DocAssociados.Application.DTOs;
using DocAssociados.Domain.Entities;
using DocAssociados.Service.Application.DTOs;
using System.Linq.Expressions;

namespace DocAssociados.Application.Interfaces;

public interface IServicoAssociado: IServico<AssociadoDto, Associado>
{
    Task<AssociadoDto> AdicionaAssociadoComEnderecoAsync(AssociadoDto associadoDto);
    Task<AssociadoDto> BuscaAssociadoComEnderecoAsync(Expression<Func<AssociadoDto, bool>> predicate);
    Task<AssociadoResumidoDto> BuscaAssociadoResumidamenteAsync(Guid id);
    Task<AssociadoResumidoDto> AtualizaAssociadoParcialmenteAsync(AssociadoResumidoDto associadoResumidoDto);
    Task DeletaAssociadoIdentity(AssociadoDto associado);
    Task DeletaAssociadoAsync(AssociadoDto associadoDto);
}
