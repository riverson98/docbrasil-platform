using DocAssociados.Domain.Interfaces;
using DocAssociados.Infra.Data.Context;

namespace DocAssociados.Infra.Data.Repository;

public class UnityOfWork : IUnityOfWork
{
    private IAssociadoRepositorio _associadoRepositorio;
    private IEnderecoRepositorio _enderecoRepositorio;
    protected readonly AppDbContext _context;

    public UnityOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IAssociadoRepositorio AssociadoRepositorio
    {
        get
        {
            return _associadoRepositorio = _associadoRepositorio ?? new AssociadoRepositorio(_context);
        }
    }

    public IEnderecoRepositorio EnderecoRepositorio
    {
        get
        {
            return _enderecoRepositorio = _enderecoRepositorio ?? new EnderecoRepositorio(_context);
        }
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
