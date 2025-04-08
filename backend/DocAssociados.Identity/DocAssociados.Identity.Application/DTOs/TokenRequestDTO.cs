using System.ComponentModel.DataAnnotations;

namespace DocAssociados.Identity.Application.DTOs;

public class TokenRequestDTO
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
