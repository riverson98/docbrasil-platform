using DocAssociados.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DocAssociados.Application.DTOs;

public class EnderecoDto : IUploadable
{
    #region Genericos
    IFormFile? IUploadable.FotoDoArquivo { get => FotoDoComprovante; set => FotoDoComprovante = value; }
    string? IUploadable.SufixoBlob { get => _blobSufixo; }
    string? IUploadable.UrlDaFoto { get => ComprovanteDeResidenciaUpload; set => ComprovanteDeResidenciaUpload = value; }
    private readonly string _blobSufixo = "Endereco";
    #endregion

    public int? Id { get; set; }
    public string Cep { get; set; }
    public string Rua { get; set; }
    public string Numero { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string? ComprovanteDeResidenciaUpload { get; set; }
    public IFormFile FotoDoComprovante {  get; set; }
    public DateTime? DataDeCadastro { get; set; } = DateTime.Now;
}
