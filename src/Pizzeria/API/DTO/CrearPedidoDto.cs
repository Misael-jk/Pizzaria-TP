using System.ComponentModel.DataAnnotations;

namespace Pizzeria.API.DTO;

public class CrearPedidoDto
{
    public int IdCliente { get; set; }
    public string DireccionEntrega { get; set; } = string.Empty;
    public List<DetalleDto> Detalles { get; set; } = new List<DetalleDto>();
}
