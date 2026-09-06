using System.Collections.Concurrent;

namespace Pizzeria.Servicios.Background;

public interface IPedidoProcessingQueue
{
    Task EnqueueAsync(int pedidoId);
    Task<int> DequeueAsync(CancellationToken cancellationToken);
}
