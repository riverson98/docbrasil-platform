using System.ComponentModel.DataAnnotations;

namespace DocAssociados.Identity.Application.DTOs;

public class UserIdentityDTO
{
    public string Id { get; set; }

    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
