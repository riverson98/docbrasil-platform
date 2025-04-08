namespace DocAssociados.Identity.Application.DTOs;

public class AuthenticationDTO
{
    public string? Message { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Email { get; set; }
    public IEnumerable<string>? Roles { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}
