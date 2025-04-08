namespace DocAssociados.Identity.Domain.Entity;

public sealed class Authorization
{
    public enum Roles
    {
        Administrator,
        Moderator,
        User
    }

    public const Roles DEFAULT_ROLE = Roles.User;
}
