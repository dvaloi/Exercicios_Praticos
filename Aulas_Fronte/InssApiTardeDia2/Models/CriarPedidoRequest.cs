namespace InssApi.Models;

public class CriarPedidoRequest
{
    public int ContribuinteId { get; set; }
    public string Tipo { get; set; } = string.Empty;
}
