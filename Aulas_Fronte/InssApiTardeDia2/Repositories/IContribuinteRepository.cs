using InssApi.Models;

namespace InssApi.Repositories;

public interface IContribuinteRepository
{
    Task<Contribuinte?> ObterPorIdAsync(int id, CancellationToken ct);
    Task<Contribuinte?> ObterPorNuitAsync(string nuit, CancellationToken ct);
    Task<IReadOnlyList<Contribuinte>> ListarAsync(CancellationToken ct);
    Task<Contribuinte?> ObterComPedidosAsync(int id, CancellationToken ct);
    Task AdicionarAsync(Contribuinte contribuinte, CancellationToken ct);
    void Remover(Contribuinte contribuinte);
    Task SalvarAsync(CancellationToken ct);
}
