using InssApi.Models;
using InssApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InssApi.Controllers;

/// <summary>Dia 2 tarde (M8) — só HTTP. Paginação e transação no Service.</summary>
[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _service;

    public PedidosController(IPedidoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 10,
        CancellationToken ct = default)
        => Ok(await _service.ListarAsync(pagina, tamanho, ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var p = await _service.ObterPorIdAsync(id, ct);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarPedidoRequest req, CancellationToken ct)
    {
        var (ok, erro, criado) = await _service.CriarAsync(req.ContribuinteId, req.Tipo, ct);
        if (!ok)
            return BadRequest(erro);

        return CreatedAtAction(nameof(GetById), new { id = criado!.Id }, criado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CriarPedidoRequest req, CancellationToken ct)
    {
        var (ok, erro, atualizado) = await _service.AtualizarAsync(id, req.ContribuinteId, req.Tipo, ct);
        if (atualizado is null && erro is null)
            return NotFound();
        if (!ok)
            return BadRequest(erro);
        return Ok(atualizado);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> PatchStatus(int id, [FromBody] string status, CancellationToken ct)
    {
        var (ok, erro, atualizado) = await _service.PatchStatusAsync(id, status, ct);
        if (atualizado is null && erro is null)
            return NotFound();
        if (!ok)
            return BadRequest(erro);
        return Ok(atualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => await _service.RemoverAsync(id, ct) ? NoContent() : NotFound();
}
