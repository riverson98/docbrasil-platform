using DocAssociados.Identity.Domain.Validation;
using System.Xml.Linq;

namespace DocAssociados.Identity.Domain.Entity;

public sealed class UserIdentity
{
    public string Id { get; private set; }
    public string? Name { get; private set; }
    public string? Email { get; private set; }
    public string? Password { get; private set; }

    public UserIdentity()
    {
        
    }

    public UserIdentity(Guid id, string? email, string? password, string? confirmPassword, string name)
    {
        ValidateDomain(id, email, password, confirmPassword, name);
    }

    private void ValidateDomain(Guid id, string? email, string? password, string? confirmPassword, string name)
    {
        DomainExceptionValidation.When(Guid.Empty.Equals(id),
            "The id is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(email),
            "The e-mail is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(password),
            "The password is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(name),
            "The password is required");

        Email = email;
        Password = password;
        Name = name;
    }
}
