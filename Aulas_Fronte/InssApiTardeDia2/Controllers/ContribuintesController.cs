using InssApi.Models;
using InssApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace InssApi.Controllers;

/// <summary>Dia 2 tarde (M8) — só HTTP. Regras no Service.</summary>
[ApiController]
[Route("api/[controller]")]
public class ContribuintesController : ControllerBase
{
    private readonly IContribuinteService _service;

    public ContribuintesController(IContribuinteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
        => Ok(await _service.ListarAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var c = await _service.ObterPorIdAsync(id, ct);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpGet("{id:int}/pedidos")]
    public async Task<IActionResult> PedidosDoContribuinte(int id, CancellationToken ct)
    {
        var pedidos = await _service.PedidosDoContribuinteAsync(id, ct);
        return pedidos is null ? NotFound() : Ok(pedidos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarContribuinteRequest req, CancellationToken ct)
    {
        var (ok, erro, criado) = await _service.CriarAsync(req.Nuit, req.Nome, ct);
        if (!ok)
            return BadRequest(erro);

        return CreatedAtAction(nameof(GetById), new { id = criado!.Id }, criado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CriarContribuinteRequest req, CancellationToken ct)
    {
        var (ok, erro, atualizado) = await _service.AtualizarAsync(id, req.Nuit, req.Nome, ct);
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
