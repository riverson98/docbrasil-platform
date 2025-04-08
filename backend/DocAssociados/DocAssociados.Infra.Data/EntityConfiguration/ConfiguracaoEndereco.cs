using DocAssociados.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocAssociados.Infra.Data.EntityConfiguration;

public class ConfiguracaoEndereco : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.HasKey(it => it.Id);
        builder.Property(it => it.Rua).HasMaxLength(100).IsRequired();
        builder.Property(it => it.Numero).IsRequired();
        builder.Property(it => it.Bairro).HasMaxLength(30).IsRequired();
        builder.Property(it => it.Estado).HasMaxLength(2).IsRequired();
        builder.Property(it => it.Cidade).HasMaxLength(20).IsRequired();
        builder.Property(it => it.Cep).HasMaxLength(15).IsRequired();
        builder.Property(it => it.ComprovanteDeResidenciaUpload);
        builder.Property(it => it.DataDoUpload);

        builder.HasOne(it => it.Associado)
               .WithOne(it => it.Endereco)
               .HasForeignKey<Endereco>(it => it.AssociadoId);
    }
}
