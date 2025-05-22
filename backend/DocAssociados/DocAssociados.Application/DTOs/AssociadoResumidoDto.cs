namespace DocAssociados.Service.Application.DTOs;

public class AssociadoResumidoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string? Email { get; set; }
    public DateOnly DataDeNascimento { get; set; }
    public string Genero { get; set; }
    public int? CodigoAssociado { get; set; }
    public int? Funcao { get; set; }
    public string? UrlFotoDePerfil { get; set; }

}
