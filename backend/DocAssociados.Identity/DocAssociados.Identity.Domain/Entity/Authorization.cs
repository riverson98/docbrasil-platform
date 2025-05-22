namespace DocAssociados.Identity.Domain.Entity;

public sealed class Authorization
{
    public enum Roles
    {
        ADMINISTRADOR
    }

    public const Roles DEFAULT_ROLE = Roles.ADMINISTRADOR;
}
