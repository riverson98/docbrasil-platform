using AutoMapper;
using DocAssociados.Application.DTOs;
using DocAssociados.Application.Interfaces;
using DocAssociados.Domain.Entities;
using DocAssociados.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace DocAssociados.Application.Services;

public class ServicoEndereco : Servico<EnderecoDto, Endereco>, IServicoEndereco
{
    public ServicoEndereco(IUnityOfWork unityOfWork, IMapper mapper, IRepositorio<Endereco> repositorio,
        IServicoAzure<EnderecoDto> servicoAzure) : base(unityOfWork, mapper, repositorio, servicoAzure)
    {}
}
