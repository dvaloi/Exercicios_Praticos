using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/stock")]
public sealed class StockController : ControllerBase
{
    private static readonly List<StockItemResponse> Items =
    [
        new(1, "Notebook", "Eletronicos", 15, 4500m),
        new(2, "Mouse", "Eletronicos", 50, 120m),
        new(3, "Teclado", "Eletronicos", 30, 250m),
        new(4, "Cadeira", "Moveis", 8, 1200m),
        new(5, "Mesa", "Moveis", 10, 900m)
    ];

    [HttpGet]
    [Authorize(Policy = "CanReadStock")]
    public IActionResult Get(
        [FromQuery] string? name,
        [FromQuery] string? category,
        [FromQuery] string sortBy = "id",
        [FromQuery] string sortDirection = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Parâmetros de paginação inválidos",
                Detail = "page deve ser >= 1 e pageSize deve estar entre 1 e 100.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        IEnumerable<StockItemResponse> query = Items;

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(x =>
                x.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x =>
                x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        query = (sortBy.ToLowerInvariant(), sortDirection.ToLowerInvariant()) switch
        {
            ("name", "desc") => query.OrderByDescending(x => x.Name),
            ("name", _)      => query.OrderBy(x => x.Name),
            ("price", "desc") => query.OrderByDescending(x => x.Price),
            ("price", _)       => query.OrderBy(x => x.Price),
            ("quantity", "desc") => query.OrderByDescending(x => x.Quantity),
            ("quantity", _)       => query.OrderBy(x => x.Quantity),
            (_, "desc") => query.OrderByDescending(x => x.Id),
            _ => query.OrderBy(x => x.Id)
        };

        var totalItems = query.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var data = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new
        {
            page,
            pageSize,
            totalItems,
            totalPages,
            data
        });
    }

    [HttpPost]
    [Authorize(Policy = "CanWriteStock")]
    public IActionResult Create(StockItemRequest request)
    {
        var nextId = Items.Count == 0 ? 1 : Items.Max(x => x.Id) + 1;

        var item = new StockItemResponse(
            nextId,
            request.Name,
            request.Category,
            request.Quantity,
            request.Price
        );

        Items.Add(item);

        Estoque.Api.Metrics.StockMetrics.CreatedItems.Inc();

        return CreatedAtAction(
            nameof(GetById),
            new { version = "1.0", id = item.Id },
            item
        );
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "CanReadStock")]
    public IActionResult GetById(int id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);

        if (item is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Item não encontrado",
                Detail = $"Nenhum item com id {id} foi encontrado.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(item);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);

        if (item is null)
        {
            return NotFound();
        }

        Items.Remove(item);
        return NoContent();
    }
}

public sealed record StockItemRequest(
    string Name,
    string Category,
    int Quantity,
    decimal Price
);

public sealed record StockItemResponse(
    int Id,
    string Name,
    string Category,
    int Quantity,
    decimal Price
);
