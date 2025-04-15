using DocAssociados.Application.Interfaces;
using DocAssociados.Service.Application.Enums;
using Microsoft.AspNetCore.Http;

namespace DocAssociados.Service.Application.DTOs;

public class AssociadoResumidoDto
{
    public Guid? Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public Status Status { get; set; }
    public Funcoes Funcao { get; set; }
}
