using InssApi.Models;

namespace InssApi.Services;

public interface IPedidoService
{
    Task<object> ListarAsync(int pagina, int tamanho, CancellationToken ct);
    Task<PedidoBeneficio?> ObterPorIdAsync(int id, CancellationToken ct);
    Task<(bool Ok, string? Erro, PedidoBeneficio? Criado)> CriarAsync(int contribuinteId, string tipo, CancellationToken ct);
    Task<(bool Ok, string? Erro, PedidoBeneficio? Atualizado)> AtualizarAsync(int id, int contribuinteId, string tipo, CancellationToken ct);
    Task<(bool Ok, string? Erro, PedidoBeneficio? Atualizado)> PatchStatusAsync(int id, string status, CancellationToken ct);
    Task<bool> RemoverAsync(int id, CancellationToken ct);
}
