namespace InssApi.Models;

/// <summary>Projeção — resumo para o GET paginado (nome da Ana, não a ficha inteira).</summary>
public record PedidoResumoDto(int Id, string Tipo, string Status, string NomeContribuinte);
