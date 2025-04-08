namespace DocAssociados.Domain.Interfaces;

public interface IUnityOfWork
{
    IAssociadoRepositorio AssociadoRepositorio { get; }
    IEnderecoRepositorio EnderecoRepositorio { get; }

    Task CommitAsync();
}
