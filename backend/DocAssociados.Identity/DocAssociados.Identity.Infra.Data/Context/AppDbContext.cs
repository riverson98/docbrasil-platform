using DocAssociados.Identity.Domain.Entity;
using DocAssociados.Identity.Infra.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DocAssociados.Identity.Infra.Data.Context;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Owned(typeof(RefreshToken));

        modelBuilder.Entity<AppUser>()
        .Property(u => u.Id)
        .ValueGeneratedNever();
    }
}
