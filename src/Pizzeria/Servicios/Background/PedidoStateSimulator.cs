using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pizzeria.Dominio.Enums;
using Pizzeria.Servicios.Interface;

namespace Pizzeria.Servicios.Background;

public class PedidoStateSimulator : BackgroundService
{
    private readonly IPedidoProcessingQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PedidoStateSimulator> _logger;

    public PedidoStateSimulator(
        IPedidoProcessingQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<PedidoStateSimulator> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PedidoStateSimulator started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            int pedidoId;
            try
            {
                pedidoId = await _queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            // Process the state machine for the dequeued pedido
            _ = Task.Run(async () => await ProcessPedidoAsync(pedidoId), stoppingToken);
        }

        _logger.LogInformation("PedidoStateSimulator stopped.");
    }

    private async Task ProcessPedidoAsync(int pedidoId)
    {
        try
        {
            // COCINA: Tomando el pedido
            await Task.Delay(5000);
            await ApplyState(pedidoId, EstadoPedido.EnPreparacion);

            // HORNO: Cocinando
            await Task.Delay(10000);
            await ApplyState(pedidoId, EstadoPedido.Listo);

            // REPARTO: En camino
            await Task.Delay(5000);
            await ApplyState(pedidoId, EstadoPedido.EnViaje);

            // ENTREGA: Finalizado
            await Task.Delay(12000);
            await ApplyState(pedidoId, EstadoPedido.Entregado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[SIMULACIÓN] Error procesando pedido {pedidoId}");
        }
    }

    private async Task ApplyState(int pedidoId, EstadoPedido nuevoEstado)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var pedidoService = scope.ServiceProvider.GetRequiredService<IPedidoService>();
            await pedidoService.CambiarEstadoPedidoAsync(pedidoId, nuevoEstado);
            _logger.LogInformation($"[SIMULACIÓN] Pedido {pedidoId} cambiado a {nuevoEstado}.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[SIMULACIÓN] Falló aplicar estado {nuevoEstado} para pedido {pedidoId}");
        }
    }
}
