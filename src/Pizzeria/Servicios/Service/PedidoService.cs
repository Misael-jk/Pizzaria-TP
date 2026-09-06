using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pizzeria.Dominio.Entidades;
using Pizzeria.Dominio.Enums;
using Pizzeria.Dominio.Interfaces;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Servicios.Service;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IServiceScopeFactory scopeFactory,
        ILogger<PedidoService> logger)
    {
        _pedidoRepository = pedidoRepository;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<int> CrearPedidoAsync(Pedido pedido)
    {
        pedido.CalcularTotal();

        if (pedido.Detalles == null || !pedido.Detalles.Any())
            throw new ArgumentException("El pedido debe contener al menos una pizza.");

        if (pedido.Total <= 0)
            throw new ArgumentException("El total del pedido no puede ser cero o negativo.");

        var idGenerado = await _pedidoRepository.CrearPedidoAsync(pedido);

        _logger.LogInformation($"[NUEVO] Pedido {idGenerado} registrado con éxito.");

        _ = Task.Run(async () => await CicloEstadosPedidoAsync(idGenerado));

        return idGenerado;
    }

    public async Task<Pedido?> ObtenerPedidoAsync(int id)
            => await _pedidoRepository.ObtenerPorIdAsync(id);

    public async Task CambiarEstadoPedidoAsync(int idPedido, EstadoPedido nuevoEstado)
    {
        var pedido = await _pedidoRepository.ObtenerPorIdAsync(idPedido);

        if (pedido == null)
            throw new KeyNotFoundException($"El pedido {idPedido} no existe.");

        await _pedidoRepository.ActualizarEstadoAsync(idPedido, nuevoEstado);
    }

    private async Task CicloEstadosPedidoAsync(int idPedido)
    {
        // Al estar en un hilo separado, necesitamos crear un "Scope" manual 
        // para obtener una instancia nueva y segura del repositorio.
        using var scope = _scopeFactory.CreateScope();

        var repo = scope.ServiceProvider.GetRequiredService<IPedidoRepository>();

        try
        {
            // COCINA: Tomando el pedido
            await Task.Delay(5000); 

            await repo.ActualizarEstadoAsync(idPedido, EstadoPedido.EnPreparacion);
            _logger.LogInformation($"[SIMULACIÓN] Pedido {idPedido} EN PREPARACIÓN.");

            // HORNO: Cocinando
            await Task.Delay(10000);

            await repo.ActualizarEstadoAsync(idPedido, EstadoPedido.Listo);
            _logger.LogInformation($"[SIMULACIÓN] Pedido {idPedido} LISTO para retirar.");

            // REPARTO: En camino
            await Task.Delay(5000);

            await repo.ActualizarEstadoAsync(idPedido, EstadoPedido.EnViaje);
            _logger.LogInformation($"[SIMULACIÓN] Pedido {idPedido} EN VIAJE.");

            // ENTREGA: Finalizado
            await Task.Delay(12000);

            await repo.ActualizarEstadoAsync(idPedido, EstadoPedido.Entregado);
            _logger.LogInformation($"[SIMULACIÓN] Pedido {idPedido} ENTREGADO.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[ERROR] Falló la simulación del pedido {idPedido}");
        }
    }
}
