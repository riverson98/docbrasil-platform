using DocAssociados.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DocAssociados.Service.Application.DTOs;

public class TermoAdesaoDto : IUploadable
{
    #region Genericos
    IFormFile? IUploadable.FotoDoArquivo { get => TermoAdesao; set => TermoAdesao = value; }
    string? IUploadable.SufixoBlob { get => _blobSufixo; }
    string? IUploadable.UrlDaFoto { get => TermoAdesaoUploadUrl; set => TermoAdesaoUploadUrl = value; }
    public bool TemArquivoParaUpload { get => TermoAdesao != null; }

    private readonly string _blobSufixo = $"Termo-adesao.docx";
    #endregion

    public IFormFile? TermoAdesao { get; set; }
    public string? TermoAdesaoUploadUrl { get; set; }
}
