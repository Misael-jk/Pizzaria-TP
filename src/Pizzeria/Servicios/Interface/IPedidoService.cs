using Pizzeria.Servicios.DTOs;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;

namespace Pizzeria.Servicios.Interface;

public interface IPedidoService
{
    Task<int> CrearPedidoAsync(CrearPedidoRequest request);
    Task<Pedido?> ObtenerPedidoAsync(int id);
    Task CambiarEstadoPedidoAsync(int idPedido, EstadoPedido nuevoEstado);
}
