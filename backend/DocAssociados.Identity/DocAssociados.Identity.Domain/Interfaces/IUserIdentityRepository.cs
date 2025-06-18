using DocAssociados.Identity.Domain.Entity;

namespace DocAssociados.Identity.Domain.Interfaces;

public interface IUserIdentityRepository
{
    Task<Response> RegisterAsync(UserIdentity user);
    Task<Authentication> GenerateAccessTokenAsync(TokenRequest request);
    Task<string> AddRoleAsync(UserToRole userToRole);
    Task<Authentication> RefreshTokenAsync(string token);
    Task<bool> UpdatePasswordAsync(UpdatePassword updatePassword);
    Task DeleteUserByIdAsync(Guid id);
    Task RevokeRefreshToken(string email);
}
