using DocAssociados.Identity.Domain.Validation;

namespace DocAssociados.Identity.Domain.Entity;

public sealed class Authentication
{
    public string? Message { get; private set; }
    public bool IsAuthenticated { get; private set; }
    public string? Email { get; private set; }
    public IEnumerable<string>? Roles { get; private set; }
    public string? Token { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime RefreshTokenExpiration { get; private set; }

    public Authentication(string? message, bool isAuthenticated, string? email, IEnumerable<string> roles,
        string? token, string? refreshToken, DateTime refreshTokenExpiration)
    {
        ValidateDomain(message, isAuthenticated, email, roles, token, refreshToken, refreshTokenExpiration);
    }

    private void ValidateDomain(string? message, bool isAuthenticated, string? email, IEnumerable<string> roles,
                                string? token, string? refreshToken, DateTime refreshTokenExpiration)
    {
        DomainExceptionValidation.When(string.IsNullOrEmpty(message),
            "The messager is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(email),
            "The email is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(token),
            "token is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(refreshToken),
            "refresh token is required");

        DomainExceptionValidation.When(refreshTokenExpiration <= DateTime.Now,
            "refresh token is expired");

        Message = message;
        IsAuthenticated = isAuthenticated;
        Email = email;
        Roles = roles;
        Token = token;
        RefreshToken = refreshToken;
        RefreshTokenExpiration = refreshTokenExpiration;
    }
}
