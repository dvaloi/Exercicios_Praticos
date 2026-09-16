using InssApi.Models;
using InssApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InssApi.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidos;
    private readonly IContribuinteRepository _contribuintes;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(
        IPedidoRepository pedidos,
        IContribuinteRepository contribuintes,
        ILogger<PedidoService> logger)
    {
        _pedidos = pedidos;
        _contribuintes = contribuintes;
        _logger = logger;
    }

    public async Task<object> ListarAsync(int pagina, int tamanho, CancellationToken ct)
    {
        if (pagina < 1) pagina = 1;
        if (tamanho < 1 || tamanho > 50) tamanho = 10;

        var query = _pedidos.Consulta();
        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderBy(p => p.Id)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .Select(p => new PedidoResumoDto(p.Id, p.Tipo, p.Status, p.Contribuinte!.Nome))
            .ToListAsync(ct);

        return new { total, pagina, tamanho, itens };
    }

    public Task<PedidoBeneficio?> ObterPorIdAsync(int id, CancellationToken ct)
        => _pedidos.ObterPorIdAsync(id, ct);

    public async Task<(bool Ok, string? Erro, PedidoBeneficio? Criado)> CriarAsync(
        int contribuinteId, string tipo, CancellationToken ct)
    {
        if (contribuinteId <= 0)
            return (false, "ContribuinteId inválido.", null);

        if (string.IsNullOrWhiteSpace(tipo))
            return (false, "Tipo é obrigatório (ex.: Velhice ou Invalidez).", null);

        if (await _contribuintes.ObterPorIdAsync(contribuinteId, ct) is null)
            return (false, "ContribuinteId inexistente.", null);

        await _pedidos.BeginTransactionAsync(ct);
        try
        {
            var p = new PedidoBeneficio
            {
                ContribuinteId = contribuinteId,
                Tipo = tipo.Trim(),
                Status = "Aberto"
            };
            await _pedidos.AdicionarAsync(p, ct);
            await _pedidos.SalvarAsync(ct);
            await _pedidos.CommitAsync(ct);
            _logger.LogInformation("Pedido criado {Id} para contribuinte {ContribuinteId}", p.Id, contribuinteId);
            return (true, null, p);
        }
        catch (Exception ex)
        {
            await _pedidos.RollbackAsync(ct);
            _logger.LogError(ex, "Falha ao gravar pedido");
            throw;
        }
    }

    public async Task<(bool Ok, string? Erro, PedidoBeneficio? Atualizado)> AtualizarAsync(
        int id, int contribuinteId, string tipo, CancellationToken ct)
    {
        var p = await _pedidos.ObterPorIdAsync(id, ct);
        if (p is null)
            return (false, null, null);

        if (contribuinteId <= 0)
            return (false, "ContribuinteId inválido.", null);

        if (string.IsNullOrWhiteSpace(tipo))
            return (false, "Tipo é obrigatório.", null);

        if (await _contribuintes.ObterPorIdAsync(contribuinteId, ct) is null)
            return (false, "ContribuinteId inexistente.", null);

        p.ContribuinteId = contribuinteId;
        p.Tipo = tipo.Trim();
        await _pedidos.SalvarAsync(ct);
        return (true, null, p);
    }

    public async Task<(bool Ok, string? Erro, PedidoBeneficio? Atualizado)> PatchStatusAsync(
        int id, string status, CancellationToken ct)
    {
        var p = await _pedidos.ObterPorIdAsync(id, ct);
        if (p is null)
            return (false, null, null);

        if (string.IsNullOrWhiteSpace(status))
            return (false, "Status é obrigatório.", null);

        p.Status = status.Trim();
        await _pedidos.SalvarAsync(ct);
        return (true, null, p);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var p = await _pedidos.ObterPorIdAsync(id, ct);
        if (p is null)
            return false;

        _pedidos.Remover(p);
        await _pedidos.SalvarAsync(ct);
        return true;
    }
}
