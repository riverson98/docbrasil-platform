namespace DocAssociados.Identity.Application.DTOs;

public class ResponseDTO
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public bool Success { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string>? Errors { get; set; } = new List<string>();
    public string Name { get; set; }
}
