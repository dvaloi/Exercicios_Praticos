using InssApi.Models;

namespace InssApi.Repositories;

public interface IPedidoRepository
{
    IQueryable<PedidoBeneficio> Consulta();
    Task<PedidoBeneficio?> ObterPorIdAsync(int id, CancellationToken ct);
    Task AdicionarAsync(PedidoBeneficio pedido, CancellationToken ct);
    void Remover(PedidoBeneficio pedido);
    Task SalvarAsync(CancellationToken ct);
    Task BeginTransactionAsync(CancellationToken ct);
    Task CommitAsync(CancellationToken ct);
    Task RollbackAsync(CancellationToken ct);
}
