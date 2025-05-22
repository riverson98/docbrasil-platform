using DocAssociados.Identity.Application.DTOs;

namespace DocAssociados.Identity.Application.Interfaces;

public interface IUserIdentityService
{
    Task<ResponseDTO> RegisterAsync(UserIdentityDTO userDto);
    Task<AuthenticationDTO> GetTokenAsync(TokenRequestDTO request);
    Task<AuthenticationDTO> RefreshTokenAsync(string token);
    Task<bool> UpdatePasswordAsync(UpdatePasswordDto updatePassword);
    Task DeleteUserIdentityAsync(Guid id);
}
