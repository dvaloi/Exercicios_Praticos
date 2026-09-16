using InssApi.Models;

namespace InssApi.Services;

public interface IContribuinteService
{
    Task<IReadOnlyList<Contribuinte>> ListarAsync(CancellationToken ct);
    Task<Contribuinte?> ObterPorIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<PedidoBeneficio>?> PedidosDoContribuinteAsync(int id, CancellationToken ct);
    Task<(bool Ok, string? Erro, Contribuinte? Criado)> CriarAsync(string nuit, string nome, CancellationToken ct);
    Task<(bool Ok, string? Erro, Contribuinte? Atualizado)> AtualizarAsync(int id, string nuit, string nome, CancellationToken ct);
    Task<(bool Ok, string? Erro, Contribuinte? Atualizado)> PatchStatusAsync(int id, string status, CancellationToken ct);
    Task<bool> RemoverAsync(int id, CancellationToken ct);
}
