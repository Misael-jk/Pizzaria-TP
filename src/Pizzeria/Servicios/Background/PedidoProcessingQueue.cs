using System.Collections.Concurrent;

namespace Pizzeria.Servicios.Background;

public class PedidoProcessingQueue : IPedidoProcessingQueue
{
    private readonly ConcurrentQueue<int> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);

    public Task EnqueueAsync(int pedidoId)
    {
        _queue.Enqueue(pedidoId);
        _signal.Release();
        return Task.CompletedTask;
    }

    public async Task<int> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);

        if (_queue.TryDequeue(out var id))
            return id;

        throw new InvalidOperationException("Queue signaled but no item available.");
    }
}
