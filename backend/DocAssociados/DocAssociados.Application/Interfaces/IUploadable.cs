using Microsoft.AspNetCore.Http;

namespace DocAssociados.Application.Interfaces;

public interface IUploadable
{
    IFormFile? FotoDoArquivo { get; set; }
    string? SufixoBlob { get; }
    string? UrlDaFoto { get; set; }
    bool temArquivoParaUpload { get; }
}
