using DocAssociados.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocAssociados.Infra.Data.EntityConfiguration;

public class ConfiguracaoAssociado : IEntityTypeConfiguration<Associado>
{
    public void Configure(EntityTypeBuilder<Associado> builder)
    {
        builder.HasKey(it => it.Id);
        builder.Property(it => it.Nome).HasMaxLength(60).IsRequired();
        builder.Property(it => it.Email).HasMaxLength(60).IsRequired();
        builder.Property(it => it.DataDeNascimento).IsRequired();
        builder.Property(it => it.Genero).HasMaxLength(1).IsRequired();
        builder.Property(it => it.Funcao).HasMaxLength(1).IsRequired();
        builder.Property(it => it.Status).HasMaxLength(1).IsRequired();
        builder.Property(it => it.CodigoRepresentante).HasMaxLength(4);
        builder.Property(it => it.CodigoRepresentanteSuperior).HasMaxLength(4).IsRequired();
        builder.Property(it => it.CodigoAssociado).IsRequired();
        builder.Property(it => it.DataDeCadastro).IsRequired();
        builder.Property(it => it.CpfUploadUrl).IsRequired();
        builder.Property(it => it.TermoDeAdessaoUploadUrl).IsRequired();
        builder.Property(it => it.FichaAssociacaoUploadUrl).IsRequired();
        builder.Property(it => it.RequerimentoJudicialUrl).IsRequired();

        //INDEX CONFIG
        builder.HasIndex(it => it.Email).IsUnique();
        builder.HasIndex(it => it.Cpf).IsUnique();
        builder.HasIndex(it => it.CodigoRepresentante).IsUnique();
        builder.HasIndex(it => it.CodigoAssociado).IsUnique();
    }
}
