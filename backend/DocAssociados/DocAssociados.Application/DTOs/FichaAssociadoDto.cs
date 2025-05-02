using DocAssociados.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DocAssociados.Service.Application.DTOs;

public class FichaAssociadoDto : IUploadable
{
    #region Genericos
    IFormFile? IUploadable.FotoDoArquivo { get => FichaAssociacao; set => FichaAssociacao = value; }
    string? IUploadable.SufixoBlob { get => _blobSufixo; }
    string? IUploadable.UrlDaFoto { get => FichaAssociacaoUploadUrl; set => FichaAssociacaoUploadUrl = value; }
    public bool temArquivoParaUpload { get => FichaAssociacao != null; }

    private readonly string _blobSufixo = $"Ficha-associacao";
    #endregion

    public IFormFile? FichaAssociacao { get; set; }
    public string? FichaAssociacaoUploadUrl { get; set; }
}
