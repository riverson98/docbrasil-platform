using DocAssociados.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DocAssociados.Service.Application.DTOs;

public class FotoDePerfilRequestDto : IUploadable
{
    #region Generics 
    public IFormFile? FotoDoArquivo { get => ArquivoDaFotoDePerfil; set => ArquivoDaFotoDePerfil = value; }
    public string? SufixoBlob => _blobSufixo;
    public string? UrlDaFoto { get => FotoDePerfilUrl; set => FotoDePerfilUrl = value; }
    public bool TemArquivoParaUpload => ArquivoDaFotoDePerfil != null;
    private readonly string _blobSufixo = $"Foto-perfil.png";
    #endregion

    public IFormFile? ArquivoDaFotoDePerfil { get; set; }
    public string? FotoDePerfilUrl { get; set; }
}
