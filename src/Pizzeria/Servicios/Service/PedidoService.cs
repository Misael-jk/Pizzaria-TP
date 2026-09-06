using Microsoft.Extensions.Logging;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.DTOs;
using Pizzeria.Servicios.Interface;
using Pizzeria.Servicios.Background;

namespace Pizzeria.Servicios.Service;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IPedidoProcessingQueue _queue;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IPizzaRepository pizzaRepository,
        IPedidoProcessingQueue queue,
        ILogger<PedidoService> logger)
    {
        _pedidoRepository = pedidoRepository;
        _pizzaRepository = pizzaRepository;
        _queue = queue;
        _logger = logger;
    }

    public async Task<int> CrearPedidoAsync(CrearPedidoRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Detalles == null || !request.Detalles.Any())
            throw new ArgumentException("El pedido debe contener al menos una pizza.");

        // Construir entidad Pedido desde DTO validando en server
        var pedido = new Pedido
        {
            IdCliente = request.IdCliente,
            DireccionEntrega = request.DireccionEntrega
        };

        foreach (var item in request.Detalles)
        {
            if (item.Cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero.");

            var pizza = await _pizzaRepository.ObtenerPorIdAsync(item.IdPizza);
            if (pizza == null)
                throw new KeyNotFoundException($"La pizza con ID {item.IdPizza} no existe.");

            if (!pizza.Disponible)
                throw new ArgumentException($"La pizza con ID {item.IdPizza} no está disponible.");

            if (pizza.Precio <= 0)
                throw new ArgumentException($"La pizza con ID {item.IdPizza} tiene un precio inválido.");

            var detalle = new DetallePedido
            {
                IdPizza = item.IdPizza,
                Cantidad = item.Cantidad,
                PrecioUnitario = pizza.Precio
            };

            pedido.AgregarDetalle(detalle);
        }

        pedido.CalcularTotal();

        if (pedido.Total <= 0)
            throw new ArgumentException("El total del pedido no puede ser cero o negativo.");

        var idGenerado = await _pedidoRepository.CrearPedidoAsync(pedido);

        _logger.LogInformation($"[NUEVO] Pedido {idGenerado} registrado con éxito.");

        // Encolar para simulación en background
        await _queue.EnqueueAsync(idGenerado);

        return idGenerado;
    }

    public async Task<Pedido?> ObtenerPedidoAsync(int id)
            => await _pedidoRepository.ObtenerPorIdAsync(id);

    public async Task CambiarEstadoPedidoAsync(int idPedido, EstadoPedido nuevoEstado)
    {
        var pedido = await _pedido_repository.ObtenerPorIdAsync(idPedido);

        if (pedido == null)
            throw new KeyNotFoundException($"El pedido {idPedido} no existe.");

        // Aplicar transición mediante métodos de dominio
        switch (nuevoEstado)
        {
            case EstadoPedido.EnPreparacion:
                pedido.IniciarPreparacion();
                break;
            case EstadoPedido.Listo:
                pedido.MarcarComoListo();
                break;
            case EstadoPedido.EnViaje:
                pedido.Enviar();
                break;
            case EstadoPedido.Entregado:
                pedido.Entregar();
                break;
            default:
                throw new ArgumentException("Estado de pedido desconocido.");
        }

        // Persistir el nuevo estado usando el repositorio (UPDATE existente)
        await _pedidoRepository.ActualizarEstadoAsync(idPedido, pedido.Estado);

        _logger.LogInformation($"[ESTADO] Pedido {idPedido} pasó a {pedido.Estado}.");
    }
}
