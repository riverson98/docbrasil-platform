using DocAssociados.Application.DTOs;
using DocAssociados.Application.Interfaces;
using DocAssociados.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocAssociados.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssociadoController : ControllerBase
    {
        private readonly IServicoAssociado _servicoAssociado;
        private readonly IServico<EnderecoDto, Endereco> _servico;
        private ILogger<AssociadoController> _logger;

        public AssociadoController(IServicoAssociado servicoAssociado, IServico<EnderecoDto, Endereco> servico, ILogger<AssociadoController> logger)
        {
            _servicoAssociado = servicoAssociado;
            _servico = servico;
            _logger = logger;
        }

        // GET api/<AssociadoController>/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AssociadoDto>> Obtem(Guid id)
        {
            _logger.LogInformation("Buscando associado com o ID: {associadoId}", id);
            var associadoDto = await _servicoAssociado.BuscaAsync(it => it.Id.Equals(id));

            if (associadoDto is null || associadoDto.Id.Equals(Guid.Empty))
            {
                _logger.LogWarning("Nenhum associado encontrado para o ID: {AssociadoId}", id);
                return NotFound("Nenhum associado encontrado com este ID");
            }

            _logger.LogDebug("Busca bem-sucedida para o ID: {AssociadoId}", id);
            return Ok(associadoDto);
        }

        [HttpGet("com-detalhes/{id:guid}", Name = "ObtemAssociadoComDetalhes")]
        public async Task<ActionResult<AssociadoDto>> ObtemAssociadoComDetalhes(Guid id)
        {
            _logger.LogInformation("Buscando associado e seu endereco com o ID: {associadoId}", id);
            var associadoDto = await _servicoAssociado.BuscaAssociadoComEnderecoAsync(it => it.Id.Equals(id));

            if (associadoDto is null || associadoDto.Id.Equals(Guid.Empty))
            {
                _logger.LogWarning("Nenhum associado encontrado para o ID: {AssociadoId}", id);
                return NotFound("Nenhum associado encontrado para este ID");
            }

            _logger.LogDebug("Busca bem-sucedida para o ID: {AssociadoId}", id);
            return Ok(associadoDto);
        }

        [HttpGet("busca-por-codigo-representante/{codigoRepresentante}")]
        public async Task<ActionResult> ObtemPorCodigoRepresentante(string codigoRepresentante)
        {
            _logger.LogInformation("Buscando associado de codigo: {CodigoRepresentante}", codigoRepresentante);
            var associadoEncontrado = await _servicoAssociado.BuscaAsync(it => it.CodigoRepresentante.Equals(codigoRepresentante));

            if (associadoEncontrado is null || associadoEncontrado.Id.Equals(Guid.Empty)) 
            {
                _logger.LogWarning("Nenhum associado encontrado para este codigo: {CodigoRepresentante}", codigoRepresentante);
                return NotFound("Nenhum associado encontrado com este codigo");
            }

            return Ok(associadoEncontrado);
        }

        // POST api/<AssociadoController>
        [HttpPost]
        public async Task<ActionResult<AssociadoDto>> Post([FromForm] AssociadoDto associadoDto)
        {
            _logger.LogInformation("Iniciando criação de associado com e-mail: {AssociadoEmail}", associadoDto.Email);

            var dtos = new List<IUploadable>
            {
                associadoDto,
                associadoDto.EnderecoDto,
                associadoDto.TermoAdesaoDto,
                associadoDto.FichaAssociadoDto
            };

            await _servicoAssociado.UploadImagemAsync(dtos, associadoDto.Email);
            
            var associadoAdicionado = await _servicoAssociado.AdicionaAssociadoComEnderecoAsync(associadoDto);

            _logger.LogInformation("Associado com o e-mail: {associadoEmail} e o ID: {AssociadoId} criado com sucesso", associadoAdicionado.Id, associadoDto.Email);

            return new CreatedAtRouteResult("ObtemAssociadoComDetalhes", new { id = associadoAdicionado.Id }, associadoAdicionado);
        }

        // PUT api/<AssociadoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AssociadoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
