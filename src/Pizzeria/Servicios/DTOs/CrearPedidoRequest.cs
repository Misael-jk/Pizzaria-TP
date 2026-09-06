namespace Pizzeria.Servicios.DTOs;

public class CrearPedidoRequest
{
    public int IdCliente { get; set; }
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<DetalleRequest> Detalles { get; set; } = new List<DetalleRequest>();
}
