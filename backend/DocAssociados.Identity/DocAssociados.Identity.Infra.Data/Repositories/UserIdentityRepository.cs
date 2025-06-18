using DocAssociados.Identity.Domain.Entity;
using DocAssociados.Identity.Domain.Interfaces;
using DocAssociados.Identity.Infra.Data.Context;
using DocAssociados.Identity.Infra.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DocAssociados.Identity.Infra.Data.Repositories;

public class UserIdentityRepository : IUserIdentityRepository
{
    private readonly UserManager<AppUser> _userManager;
    private readonly Jwt _jwt;
    private readonly AppDbContext _context;

    public UserIdentityRepository(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager,
                          IOptions<Jwt> jwt, AppDbContext context)
    {
        _userManager = userManager;
        _jwt = jwt.Value;
        _context = context;
    }

    public async Task<Authentication> GenerateAccessTokenAsync(TokenRequest tokenRequest)
    {
        var user = await _userManager.FindByEmailAsync(tokenRequest.Email!);

        if (user is null)
        {
            return new Authentication
                (
                    message: $"No Accounts Registered with {tokenRequest.Email}.",
                    isAuthenticated: false,
                    email: tokenRequest.Email,
                    roles: new List<string>(),
                    token: "invalid token",
                    refreshToken: "invalid refreshToken",
                    refreshTokenExpiration: DateTime.Now.AddSeconds(1),
                    name: "empty value"
                );
        }

        if (await _userManager.CheckPasswordAsync(user, tokenRequest.Password!))
        {
            var jwtSecurityToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            if (user.RefreshTokens.Any(refresh => refresh.IsActive))
                user.RefreshTokens.Where(token => token.IsActive == true)
                                                           .FirstOrDefault();
            else
            {
                var refreshToken = CreateRefreshToken();

                var auth = GenerateAuthenticationResponse("Token generated successfully", true,
                                user.Email!, roles, new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                                refreshToken.Token!, refreshToken.Expires, user.Name);

                user.RefreshTokens.Add(refreshToken);
                _context.Update(user);
                _context.SaveChanges();

                return auth;
            }
            var validRefresh = user.RefreshTokens.Where(token => token.IsActive == true)
                                                 .FirstOrDefault();

            return GenerateAuthenticationResponse("Refresh token is actived successfully", true,
                     user.Email!, roles, new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    validRefresh!.Token!, validRefresh.Expires, user.Name);
        }
        return GenerateAuthenticationResponse($"Incorrect Credentials for user {user.Email}.", false,
                tokenRequest.Email!, new List<string>(), "empty value",
                "empty value", DateTime.Now.AddSeconds(5), "empty value");
    }

    public async Task<string> AddRoleAsync(UserToRole userToRole)
    {
        var user = await _userManager.FindByEmailAsync(userToRole.Email!);
        if (user is null)
        {
            return $"No Accounts Registered with {userToRole.Email}.";
        }
        if (await _userManager.CheckPasswordAsync(user, userToRole.Password!))
        {
            var roleExists = Enum.GetNames(typeof(Authorization.Roles))
                                 .Any(role => role.ToLower().Equals(userToRole.Role!.ToLower()));
            if (roleExists)
            {
                var validRole = Enum.GetValues(typeof(Authorization.Roles))
                                    .Cast<Authorization.Roles>()
                                    .Where(role =>
                                           role.ToString().Equals(userToRole.Role,
                                                                  StringComparison.OrdinalIgnoreCase))
                                    .FirstOrDefault();

                await _userManager.AddToRoleAsync(user, validRole.ToString());

                return $"Added {userToRole.Role} to user {userToRole.Email}.";
            }
            return $"Role {userToRole.Role} not found.";
        }
        return $"Incorrect Credentials for user {user.Email}.";
    }

    public async Task<Response> RegisterAsync(UserIdentity user)
    {
        var identityUser = new AppUser
        {
            Id = user.Id,
            UserName = user.Email,
            Email = user.Email,
            Name = user.Name
        };

        var userWithSameEmail = await _userManager.FindByEmailAsync(user.Email!);

        if (userWithSameEmail is null)
        {
            var result = await _userManager.CreateAsync(identityUser, user.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(identityUser, Authorization.DEFAULT_ROLE.ToString());
                return new Response(result.Succeeded, identityUser.Id, identityUser.Email, identityUser.Name);
            }

            var errors = new List<string>();

            foreach (var error in result.Errors)
            {
                errors.Add(error.Description);
            }

            return new Response(errors, result.Succeeded);
        }
        else
        {
            var errors = new List<string>();
            errors.Add("user already registered");

            return new Response(errors, success: false);
        }
    }

    private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var roleClaims = new List<Claim>();

        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        new Claim("uid", user.Id)
    }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key!));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }

    private RefreshToken CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(randomNumber);

            return new RefreshToken
                (
                    token: Convert.ToBase64String(randomNumber),
                    expires: DateTime.UtcNow.AddMinutes(_jwt.RefreshTokenDurationInMinutes),
                    created: DateTime.UtcNow,
                    revoked: null
                );

        }
    }

    public async Task<Authentication> RefreshTokenAsync(string token)
    {
        var user = _context.Users.SingleOrDefault(user => user.RefreshTokens
                                                      .Any(refresh => refresh.Token!.Equals(token)));
        if (user is null)
            return GenerateAuthenticationResponse("Token did not match any users.", false,
                "empty value", new List<string>(), "empty value", "empty value", DateTime.Now.AddSeconds(5), "empty value");

        var refreshToken = user.RefreshTokens.Single(userToken => userToken.Token!.Equals(token));

        if (!refreshToken.IsActive)
            return GenerateAuthenticationResponse("Token Not Active", false,
               "empty value", new List<string>(), "empty value", "empty value", DateTime.Now.AddSeconds(5), "empty value");

        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        var jwtToken = await CreateJwtToken(user);

        return GenerateAuthenticationResponse("Token generated successfully", true,
               user.Email!, roles, new JwtSecurityTokenHandler().WriteToken(jwtToken),
               refreshToken.Token!, refreshToken.Expires, user.Name);

    }

    private Authentication GenerateAuthenticationResponse(string message, bool isAuthenticated, string email,
                                                          IEnumerable<string> roles, string token,
                                                          string refreshtoken, DateTime refreshTokenExpiration, string name)
    {
        return new Authentication
               (
                   message: message,
                   isAuthenticated: isAuthenticated,
                   email: email,
                   roles: roles,
                   token: token,
                   refreshToken: refreshtoken,
                   refreshTokenExpiration: refreshTokenExpiration,
                   name: name
               );
    }

    public async Task<bool> UpdatePasswordAsync(UpdatePassword updatePassword)
    {
        var user = await _userManager.FindByIdAsync(updatePassword.UserId);

        if (user == null)
            return false;

        var result = await _userManager.ChangePasswordAsync(user, updatePassword.CurrentPassword, updatePassword.NewPassword);

        return result.Succeeded;
    }

    public async Task DeleteUserByIdAsync(Guid id)
    {
        var userIdentityFound = await _userManager.FindByIdAsync(id.ToString());

        if (userIdentityFound == null)
            throw new ArgumentNullException("Sorry we can't found this user in our database");

        await _userManager.DeleteAsync(userIdentityFound);
    }

    public async Task RevokeRefreshToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return;

        user.RefreshTokens.RemoveAll(token => token.IsActive);
        await _userManager.UpdateAsync(user);
    }
}
