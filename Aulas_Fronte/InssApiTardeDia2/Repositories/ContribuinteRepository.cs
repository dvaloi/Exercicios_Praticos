using InssApi.Data;
using InssApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InssApi.Repositories;

/// <summary>Caderno — só fala com o inss.db.</summary>
public class ContribuinteRepository : IContribuinteRepository
{
    private readonly AppDbContext _db;

    public ContribuinteRepository(AppDbContext db) => _db = db;

    public Task<Contribuinte?> ObterPorIdAsync(int id, CancellationToken ct)
        => _db.Contribuintes.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Contribuinte?> ObterPorNuitAsync(string nuit, CancellationToken ct)
        => _db.Contribuintes.FirstOrDefaultAsync(c => c.Nuit == nuit, ct);

    public async Task<IReadOnlyList<Contribuinte>> ListarAsync(CancellationToken ct)
        => await _db.Contribuintes.AsNoTracking().ToListAsync(ct);

    public Task<Contribuinte?> ObterComPedidosAsync(int id, CancellationToken ct)
        => _db.Contribuintes.AsNoTracking()
            .Include(x => x.Pedidos)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task AdicionarAsync(Contribuinte contribuinte, CancellationToken ct)
    {
        _db.Contribuintes.Add(contribuinte);
        return Task.CompletedTask;
    }

    public void Remover(Contribuinte contribuinte)
        => _db.Contribuintes.Remove(contribuinte);

    public Task SalvarAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
