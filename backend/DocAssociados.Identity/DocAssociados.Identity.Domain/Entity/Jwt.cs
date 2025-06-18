namespace DocAssociados.Identity.Domain.Entity;

public sealed class Jwt
{
    public string? Key { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public double DurationInMinutes { get; set; }
    public int RefreshTokenDurationInMinutes { get; set; }
}
