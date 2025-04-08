using DocAssociados.Identity.Domain.Validation;

namespace DocAssociados.Identity.Domain.Entity;

public sealed class TokenRequest
{
    public string? Email { get; private set; }
    public string? Password { get; private set; }


    public TokenRequest(string? email, string? password)
    {
        ValidateDomain(email, password);
    }

    private void ValidateDomain(string? email, string? password)
    {
        DomainExceptionValidation.When(string.IsNullOrEmpty(email),
            "The email is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(password),
            "The password is required");

        Email = email;
        Password = password;
    }
}
