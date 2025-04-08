using DocAssociados.Identity.Domain.Validation;

namespace DocAssociados.Identity.Domain.Entity;

public sealed class RefreshToken
{
    public string? Token { get; private set; }
    public DateTime Expires { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; private set; }
    public DateTime? Revoked { get; private set; }
    public bool IsActive => Revoked == null && !IsExpired;

    public RefreshToken(string? token, DateTime expires, DateTime created, DateTime? revoked)
    {
        ValidateDomain(token, expires, created, revoked);
    }

    public void Update(string? token, DateTime expires, DateTime created, DateTime? revoked)
    {
        ValidateDomain(token, expires, created, revoked);
    }
    private void ValidateDomain(string? token, DateTime expires, DateTime created, DateTime? revoked)
    {
        DomainExceptionValidation.When(string.IsNullOrEmpty(token),
            "The token is required");

        Token = token;
        Expires = expires;
        Created = created;
        Revoked = revoked;
    }
}
