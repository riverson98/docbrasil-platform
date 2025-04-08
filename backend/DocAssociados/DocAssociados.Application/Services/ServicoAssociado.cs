using AutoMapper;
using DocAssociados.Application.DTOs;
using DocAssociados.Application.Extensions;
using DocAssociados.Application.Interfaces;
using DocAssociados.Domain.Entities;
using DocAssociados.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DocAssociados.Application.Services;

public class ServicoAssociado : Servico<AssociadoDto, Associado>, IServicoAssociado
{
    private readonly IAssociadoRepositorio _associadoRepositorio;
    public ServicoAssociado(IUnityOfWork unityOfWork, IMapper mapper, IRepositorio<Associado> repositorio,
        IServicoAzure<AssociadoDto> servicoAzure, IAssociadoRepositorio associadoRepositorio) : base(unityOfWork, mapper, repositorio, servicoAzure)
    {
        _associadoRepositorio = associadoRepositorio;
    }

    public async Task<AssociadoDto> AdicionaAssociadoComEnderecoAsync(AssociadoDto associadoDto)
    {
        var EntidadeAssociado = _mapper.Map<Associado>(associadoDto);
        var associadoAdicionado = _associadoRepositorio.AdicionaAssociadoComEndereco(EntidadeAssociado);

        await _unityOfWork.CommitAsync();

        return _mapper.Map<AssociadoDto>(associadoAdicionado);
    }

    public async Task<AssociadoDto> BuscaAssociadoComEnderecoAsync(Expression<Func<AssociadoDto, bool>> predicate)
    {
        Expression<Func<Associado, bool>> predicatoDaEntidade = predicate.ConvertExpression<AssociadoDto, Associado>();

        var associadoEncontrado = await _associadoRepositorio.BuscaAssociadoComEndereco(predicatoDaEntidade);

        return _mapper.Map<AssociadoDto>(associadoEncontrado);
    }
}
