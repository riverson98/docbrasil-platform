using System.ComponentModel.DataAnnotations;

namespace DocAssociados.Identity.Application.DTOs;

public class UpdatePasswordDto
{
    public string UserId { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}
