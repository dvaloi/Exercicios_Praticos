using InssApi.Data;
using InssApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace InssApi.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _tx;

    public PedidoRepository(AppDbContext db) => _db = db;

    public IQueryable<PedidoBeneficio> Consulta()
        => _db.Pedidos.AsNoTracking().Include(p => p.Contribuinte);

    public Task<PedidoBeneficio?> ObterPorIdAsync(int id, CancellationToken ct)
        => _db.Pedidos.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task AdicionarAsync(PedidoBeneficio pedido, CancellationToken ct)
    {
        _db.Pedidos.Add(pedido);
        return Task.CompletedTask;
    }

    public void Remover(PedidoBeneficio pedido)
        => _db.Pedidos.Remove(pedido);

    public Task SalvarAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct)
        => _tx = await _db.Database.BeginTransactionAsync(ct);

    public async Task CommitAsync(CancellationToken ct)
    {
        if (_tx is not null)
            await _tx.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct)
    {
        if (_tx is not null)
            await _tx.RollbackAsync(ct);
    }
}
