using DocAssociados.Application.Config;
using DocAssociados.Application.DTOs;
using DocAssociados.Application.Interfaces;
using DocAssociados.Domain.Entities;
using DocAssociados.Service.Application.DTOs;
using DocAssociados.Service.Application.Enums;
using DocAssociados.Service.Infra.CrossCutting.Logs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocAssociados.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssociadoController : ControllerBase
    {
        private readonly IServicoAssociado _servicoAssociado;
        private ILoggerService _logger;

        public AssociadoController(IServicoAssociado servicoAssociado, ILoggerService logger)
        {
            _servicoAssociado = servicoAssociado;
            _logger = logger;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AssociadoDto>> Obtem(Guid id)
        {
            _logger.Info($"Buscando associado com o ID: {id}");

            var associadoDto = await _servicoAssociado.BuscaAsync(it => it.Id.Equals(id));

            if (associadoDto is null || associadoDto.Id.Equals(Guid.Empty))
            {
                _logger.Info($"Nenhum associado encontrado para o ID: {id}");
                return NotFound("Nenhum associado encontrado com este ID");
            }

            _logger.Info($"Busca bem-sucedida para o ID: {id}");
            return Ok(associadoDto);
        }

        [HttpGet("associado-resumido/{id:guid}")]
        public async Task<ActionResult<AssociadoResumidoDto>> ObtemResumidamentePorId(Guid id)
        {
            _logger.Info($"Buscando associado resumido com o ID: {id}");
            var usuarioEncontrado = await _servicoAssociado.BuscaAssociadoResumidamenteAsync(id);

            if (usuarioEncontrado == null)
            {
                _logger.Info($"Nenhum associado encontrado para o ID: {id}");
                return NotFound($"Nenhum associado encontrado para este ID: {id}");
            }

            _logger.Info($"Busca bem-sucedida para o ID: {id}");
            var urlFoto = await _servicoAssociado.AtualizaTokenSas(usuarioEncontrado.UrlFotoDePerfil);

            if (!string.IsNullOrEmpty(urlFoto))
            {
                usuarioEncontrado.UrlFotoDePerfil = urlFoto;
                await _servicoAssociado.AtualizaAssociadoParcialmenteAsync(usuarioEncontrado);
            }
            
            return Ok(usuarioEncontrado);
        }

        [HttpGet("com-detalhes/{id:guid}", Name = "ObtemAssociadoComDetalhes")]
        public async Task<ActionResult<AssociadoDto>> ObtemAssociadoComDetalhes(Guid id)
        {
            _logger.Info($"Buscando associado e seu endereco com o ID: {id}");
            var associadoDto = await _servicoAssociado.BuscaAssociadoComEnderecoAsync(it => it.Id.Equals(id));

            if (associadoDto is null || associadoDto.Id.Equals(Guid.Empty))
            {
                _logger.Info("Nenhum associado encontrado para o ID: {id}");
                return NotFound("Nenhum associado encontrado para este ID");
            }

            _logger.Info($"Busca bem-sucedida para o ID: {id}");
            return Ok(associadoDto);
        }

        [HttpGet("busca-por-codigo-representante/{codigoRepresentante}")]
        public async Task<ActionResult> ObtemPorCodigoRepresentante(string codigoRepresentante)
        {
            _logger.Info($"Buscando associado de codigo: {codigoRepresentante}");
            var associadoEncontrado = await _servicoAssociado.BuscaAsync(it => it.CodigoRepresentante.Equals(codigoRepresentante));

            if (associadoEncontrado is null || associadoEncontrado.Id.Equals(Guid.Empty)) 
            {
                _logger.Info($"Nenhum associado encontrado para este codigo: {codigoRepresentante}");
                return NotFound("Nenhum associado encontrado com este codigo");
            }

            _logger.Info($"Associado de codigo de {codigoRepresentante} encontrado");

            return Ok(associadoEncontrado);
        }

        [HttpGet("busca-associados")]
        public async Task<ActionResult<ResultadoPaginado<Associado>>>BuscaAssociados([FromQuery] ParametrosDaPaginacao parametros)
        {
            _logger.Info($"Buscando associados");

            Expression<Func<AssociadoDto, bool>>? filtro = null;

            if (!string.IsNullOrEmpty(parametros.Query))
            {
                filtro = it => it.Nome.Contains(parametros.Query) ||
                                   it.Email.Contains(parametros.Query);
            }

            var associadosDto = await _servicoAssociado.BuscaDtoComPaginacaoAsync(parametros,
                it => it.Funcao.Equals((int)Funcoes.ASSOCIADO), filtro);

            if (!associadosDto.Itens.Any())
            {
                _logger.Info($"Nenhum associado encontrado");
                return NotFound("Nenhum associado encontrado");
            }

            return Ok(associadosDto);
        }

        [HttpGet("busca-membros")]
        public async Task<ActionResult<ResultadoPaginado<AssociadoResumidoDto>>> BuscaMembros([FromQuery] ParametrosDaPaginacao parametros)
        {
            _logger.Info($"Buscando membros");

            Expression<Func<AssociadoDto, bool>>? filtro = null;

            if (!string.IsNullOrEmpty(parametros.Query))
            {
                filtro = it => it.Nome.Contains(parametros.Query) ||
                                   it.Email.Contains(parametros.Query);
            }

            var membrosDto = await _servicoAssociado.BuscaDtoComPaginacaoAsync(parametros,
                        it => it.Funcao.Equals((int)Funcoes.REPRESENTANTE)
                           || it.Funcao.Equals((int)Funcoes.ADMINISTRADOR) 
                           || it.Funcao.Equals((int)Funcoes.DIRETOR),
                        filtro);

            if (!membrosDto.Itens.Any())
            {
                _logger.Info($"Nenhum representante encontrado");
                return NotFound("Nenhum representante encontrado");
            }

            return Ok(membrosDto);
        }

        [HttpPost("cria-associado")]
        public async Task<ActionResult<AssociadoDto>> Post([FromForm] AssociadoDto associadoDto)
        {
            _logger.Info($"Iniciando criação de associado com e-mail: {associadoDto.Email}");

            var dtos = new List<IUploadable>
            {
                associadoDto,
                associadoDto.EnderecoDto,
                associadoDto.TermoAdesaoDto,
                associadoDto.FichaAssociadoDto,
                associadoDto.RequerimentoJudicialDto
                
            };

            await _servicoAssociado.UploadImagemAsync(dtos, associadoDto.Email);
            
            var associadoAdicionado = await _servicoAssociado.AdicionaAssociadoComEnderecoAsync(associadoDto);

            _logger.Info($"Associado com o e-mail: {associadoDto.Email} e o ID: {associadoAdicionado.Id} criado com sucesso");

            return new CreatedAtRouteResult("ObtemAssociadoComDetalhes", new { id = associadoAdicionado.Id }, associadoAdicionado);
        }

        [HttpPost("adiciona-foto-de-perfil/{id:guid}")]
        public async Task<ActionResult<AssociadoResumidoDto>> AdicionaFotoDePerfil(Guid id, [FromForm] FotoDePerfilRequestDto foto)
        {
            var associadoEncontrado = await _servicoAssociado.BuscaAssociadoResumidamenteAsync(id);

            if (associadoEncontrado == null)
                return NotFound("Nenhum associado encontrado para esse id");

            if (foto.ArquivoDaFotoDePerfil == null)
                return BadRequest("O arquivo não pode ser nulo");

            var dtos = new List<IUploadable> { foto };

            await _servicoAssociado.UploadImagemAsync(dtos, associadoEncontrado.Email);

            var urlFoto = dtos.Select(it => it.UrlDaFoto).FirstOrDefault();

            if (!string.IsNullOrEmpty(urlFoto))
                associadoEncontrado.UrlFotoDePerfil = urlFoto;

            var associadoAtualizado = await _servicoAssociado.AtualizaAssociadoParcialmenteAsync(associadoEncontrado);

            return Ok(associadoAtualizado);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<AssociadoDto>> Put([FromForm] AssociadoDto associadoDto, Guid id)
        {
            if (!associadoDto.Id.Equals(id))
                return BadRequest("O identificador do usuário não pode ser diferente");

            if (associadoDto.FotoDoDocumento != null || associadoDto?.EnderecoDto?.FotoDoComprovante != null
                || associadoDto?.TermoAdesaoDto?.TermoAdesao != null || associadoDto?.FichaAssociadoDto?.FichaAssociacao != null)
            {
                var dtos = new List<IUploadable>
                {   
                    associadoDto,
                    associadoDto.EnderecoDto,
                    associadoDto.TermoAdesaoDto,
                    associadoDto.FichaAssociadoDto,
                    associadoDto.RequerimentoJudicialDto
                }
                .Where(it => it != null && it.TemArquivoParaUpload)
                .ToList();

                await _servicoAssociado.UploadImagemAsync(dtos, associadoDto.Email);
            }

            var associadoAtualizado = await _servicoAssociado.AtualizaAsync(associadoDto);

            return new CreatedAtRouteResult("ObtemAssociadoComDetalhes", new { id = associadoAtualizado.Id }, associadoAtualizado);
        }

        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<AssociadoResumidoDto>> Patch(Guid id, JsonPatchDocument<AssociadoResumidoDto> associadoResumidoDto)
        {
            if (associadoResumidoDto == null)
                return BadRequest("As informações do usuário não podem ser nulas");

            var associado = await _servicoAssociado.BuscaAssociadoResumidamenteAsync(id);

            if (associado == null)
                return NotFound("Nenhum usuário encontrado");

            associadoResumidoDto.ApplyTo(associado);

            var associadoAtualizado = await _servicoAssociado.AtualizaAssociadoParcialmenteAsync(associado);
            
            return Ok(associadoAtualizado);
        }

        [HttpDelete("deleta-associado/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var associadoEncontrado = await _servicoAssociado.BuscaAssociadoComEnderecoAsync(it => it.Id.Equals(id));

            if (associadoEncontrado == null)
                return BadRequest("Nenhum associado encontrado para este id");

            await _servicoAssociado.DeletaAssociadoAsync(associadoEncontrado);
            await _servicoAssociado.DeletaAssociadoIdentity(associadoEncontrado);

            return NoContent();
        }
    }
}
