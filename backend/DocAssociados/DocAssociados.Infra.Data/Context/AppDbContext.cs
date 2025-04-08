using DocAssociados.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DocAssociados.Infra.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Associado> Associado { get; set; }
    public DbSet<Endereco> Endereco {  get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
