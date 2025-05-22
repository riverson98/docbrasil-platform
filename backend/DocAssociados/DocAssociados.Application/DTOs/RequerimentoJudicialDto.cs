using DocAssociados.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DocAssociados.Service.Application.DTOs;

public class RequerimentoJudicialDto : IUploadable
{
    #region Genericos
    IFormFile? IUploadable.FotoDoArquivo { get => ArquivoDoRequerimento; set => ArquivoDoRequerimento = value; }
    string? IUploadable.SufixoBlob { get => _blobSufixo; }
    string? IUploadable.UrlDaFoto { get => UrlDoRequerimento; set => UrlDoRequerimento = value; }
    public bool TemArquivoParaUpload { get => ArquivoDoRequerimento != null; }

    private readonly string _blobSufixo = $"Requerimento-Judicial.docx";
    #endregion
    public IFormFile? ArquivoDoRequerimento { get; set; }
    public string? UrlDoRequerimento { get; set; }
}
