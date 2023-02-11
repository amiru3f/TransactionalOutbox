using TransactionalOutbox.Services;

namespace TransactionalOutbox;

public sealed class MockBus : IBus
{
    public int SentCount { get => count; }
    private int count = 0;
    public Task PublishAsync(object id)
    {
        Interlocked.Increment(ref count);
        return Task.CompletedTask;
    }
}