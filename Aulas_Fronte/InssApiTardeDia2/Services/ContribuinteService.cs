using InssApi.Models;
using InssApi.Repositories;

namespace InssApi.Services;

/// <summary>Cérebro — regras da Ana. Não HTTP, não SQL.</summary>
public class ContribuinteService : IContribuinteService
{
    private readonly IContribuinteRepository _repo;
    private readonly ILogger<ContribuinteService> _logger;

    public ContribuinteService(IContribuinteRepository repo, ILogger<ContribuinteService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public Task<IReadOnlyList<Contribuinte>> ListarAsync(CancellationToken ct)
        => _repo.ListarAsync(ct);

    public Task<Contribuinte?> ObterPorIdAsync(int id, CancellationToken ct)
        => _repo.ObterPorIdAsync(id, ct);

    public async Task<IReadOnlyList<PedidoBeneficio>?> PedidosDoContribuinteAsync(int id, CancellationToken ct)
    {
        var c = await _repo.ObterComPedidosAsync(id, ct);
        return c?.Pedidos;
    }

    public async Task<(bool Ok, string? Erro, Contribuinte? Criado)> CriarAsync(
        string nuit, string nome, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(nuit) || nuit.Length != 9)
            return (false, "NUIT deve ter 9 dígitos.", null);

        if (string.IsNullOrWhiteSpace(nome))
            return (false, "Nome é obrigatório.", null);

        if (await _repo.ObterPorNuitAsync(nuit, ct) is not null)
        {
            _logger.LogWarning("NUIT duplicado {Nuit}", nuit);
            return (false, "NUIT já cadastrado.", null);
        }

        var c = new Contribuinte { Nuit = nuit.Trim(), Nome = nome.Trim() };
        await _repo.AdicionarAsync(c, ct);
        await _repo.SalvarAsync(ct);
        _logger.LogInformation("Contribuinte criado {Nuit} {Nome}", c.Nuit, c.Nome);
        return (true, null, c);
    }

    public async Task<(bool Ok, string? Erro, Contribuinte? Atualizado)> AtualizarAsync(
        int id, string nuit, string nome, CancellationToken ct)
    {
        var c = await _repo.ObterPorIdAsync(id, ct);
        if (c is null)
            return (false, null, null);

        if (string.IsNullOrWhiteSpace(nuit) || nuit.Length != 9)
            return (false, "NUIT deve ter 9 dígitos.", null);

        if (string.IsNullOrWhiteSpace(nome))
            return (false, "Nome é obrigatório.", null);

        c.Nuit = nuit.Trim();
        c.Nome = nome.Trim();
        await _repo.SalvarAsync(ct);
        return (true, null, c);
    }

    public async Task<(bool Ok, string? Erro, Contribuinte? Atualizado)> PatchStatusAsync(
        int id, string status, CancellationToken ct)
    {
        var c = await _repo.ObterPorIdAsync(id, ct);
        if (c is null)
            return (false, null, null);

        if (string.IsNullOrWhiteSpace(status))
            return (false, "Status é obrigatório.", null);

        c.Status = status.Trim();
        await _repo.SalvarAsync(ct);
        return (true, null, c);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var c = await _repo.ObterPorIdAsync(id, ct);
        if (c is null)
            return false;

        _repo.Remover(c);
        await _repo.SalvarAsync(ct);
        return true;
    }
}
