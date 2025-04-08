using DocAssociados.Application.Interfaces;
using DocAssociados.Service.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace DocAssociados.Application.DTOs;

public class AssociadoDto : IUploadable
{
    #region Genericos
    IFormFile? IUploadable.FotoDoArquivo { get => FotoDoDocumento; set => FotoDoDocumento = value; }
    string? IUploadable.SufixoBlob { get => _blobSufixo; }
    string? IUploadable.UrlDaFoto { get => CpfUploadUrl; set => CpfUploadUrl = value; }
    private readonly string _blobSufixo = "Documento";
    #endregion

    private string genero { get; set; } = string.Empty;

    public Guid? Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public DateOnly DataDeNascimento { get; set; }
    public string Genero { 
        get
        {
            return genero;
        }
        
        set 
        {
            genero = string.IsNullOrEmpty(value) ? string.Empty : value.Substring(0, 1).ToUpper();
        }
    }
    public string Cpf { get; set; }
    public IFormFile FotoDoDocumento { get; set; }
    public string? CpfUploadUrl { get; set; }
    public string? TermoDeAdessaoUploadUrl { get; set; }
    public string CodigoRepresentante { get; set; }
    public string? CodigoRepresentanteSuperior { get; set; }
    public EnderecoDto EnderecoDto { get; set; }
    public FichaAssociadoDto FichaAssociadoDto { get; set; }
    public TermoAdesaoDto TermoAdesaoDto { get; set; }
    public DateTime? DataDeCadastro { get; set; } = DateTime.Now;
}
