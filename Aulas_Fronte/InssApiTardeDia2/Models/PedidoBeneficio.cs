namespace InssApi.Models;

/// <summary>Pedido ligado à ficha (FK + navegação).</summary>
public class PedidoBeneficio
{
    public int Id { get; set; }
    public int ContribuinteId { get; set; }
    public Contribuinte? Contribuinte { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Status { get; set; } = "Aberto";
}
