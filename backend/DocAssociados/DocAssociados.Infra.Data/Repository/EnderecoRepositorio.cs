using DocAssociados.Domain.Entities;
using DocAssociados.Domain.Interfaces;
using DocAssociados.Infra.Data.Context;

namespace DocAssociados.Infra.Data.Repository;

public class EnderecoRepositorio : Repositorio<Endereco>, IEnderecoRepositorio
{
    public EnderecoRepositorio(AppDbContext context) : base(context)
    {
    }
}
