using DocAssociados.Identity.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace DocAssociados.Identity.Infra.Data.Identity;

public class AppUser : IdentityUser
{
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public string Name { get; set; }
}
