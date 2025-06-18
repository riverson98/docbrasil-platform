using DocAssociados.Application.DTOs;
using DocAssociados.Application.Interfaces;
using DocAssociados.Application.Services;
using DocAssociados.Service.Infra.CrossCutting.Logs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocAssociados.Services.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Policy = "AdminsOnly")]
    public class EnderecoController : ControllerBase
    {
        private readonly IServicoEndereco _servicoEndereco;
        private ILoggerService _logger;

        public EnderecoController(IServicoEndereco servicoEndereco, ILoggerService logger)
        {
            _servicoEndereco = servicoEndereco;
            _logger = logger;
        }

        [HttpGet("busca-endereco/{id:guid}")]
        public async Task<ActionResult<EnderecoDto>> BuscaEnderecoDoAssociado(Guid id)
        {
            _logger.Info($"Buscando o endereco do associado com o ID: {id}");

            var enderecoDto = await _servicoEndereco.BuscaAsync(it => it.AssociadoId.Equals(id));

            if (enderecoDto == null) 
            {
                _logger.Info($"Nenhum endereço encontrado para esse associado. id: {id}");
                return NotFound("Nenhum endereço encontrado para esse associado.");
            }

            _logger.Info($"Endereco encontrado com o id: {enderecoDto.Id}");
            return Ok(enderecoDto);
        }
    }
}
