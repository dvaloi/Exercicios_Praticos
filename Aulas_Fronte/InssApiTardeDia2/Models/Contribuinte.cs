namespace InssApi.Models;

/// <summary>Ficha do contribuinte — Dia 2 manhã: ganha a gaveta de pedidos (1:N).</summary>
public class Contribuinte
{
    public int Id { get; set; }
    public string Nuit { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateOnly? DataNascimento { get; set; }
    public string Status { get; set; } = "Ativo";

    public List<PedidoBeneficio> Pedidos { get; set; } = new();
}
