using DocAssociados.Identity.Domain.Validation;

namespace DocAssociados.Identity.Domain.Entity
{
    public class UserToRole
    {
        public string? Email { get; private set; }
        public string? Password { get; private set; }
        public string? Role { get; private set; }

        public UserToRole(string? email, string? password, string? role)
        {
            ValidateDomain(email, password, role);
        }

        private void ValidateDomain(string? email, string? password, string? role)
        {
            DomainExceptionValidation.When(string.IsNullOrEmpty(email),
                "The email is required");

            DomainExceptionValidation.When(string.IsNullOrEmpty(password),
                "The password is required");

            DomainExceptionValidation.When(string.IsNullOrEmpty(role),
                "The role is required");

            Email = email;
            Password = password;
            Role = role;
        }
    }
}
