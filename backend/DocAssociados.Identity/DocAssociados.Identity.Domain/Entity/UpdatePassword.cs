using DocAssociados.Identity.Domain.Validation;

namespace DocAssociados.Identity.Domain.Entity;

public sealed class UpdatePassword
{
    public string UserId { get; private set; }
    public string CurrentPassword { get; private set; }
    public string NewPassword { get; private set; }
    public string ConfirmNewPassword { get; private set; }

    public UpdatePassword()
    {
        
    }

    public UpdatePassword(string userId, string currentPassword, string newPassword, string confirmNewPassword)
    {
        ValidateDomain(userId, currentPassword, newPassword, confirmNewPassword);
    }

    private void ValidateDomain(string userId, string currentPassword, string newPassword, string confirmNewPassword)
    {
        DomainExceptionValidation.When(string.IsNullOrEmpty(userId),
            "The user id is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(currentPassword),
            "The currentPassword is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(newPassword),
            "The new passoword is required");

        DomainExceptionValidation.When(string.IsNullOrEmpty(confirmNewPassword),
            "The confirm password is required");
        
        DomainExceptionValidation.When(!newPassword.Equals(confirmNewPassword),
            "the passwords don't match");

        UserId = userId;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
        ConfirmNewPassword = confirmNewPassword;
    }
}
