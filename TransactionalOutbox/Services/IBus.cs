namespace TransactionalOutbox.Services;

public interface IBus
{
    Task PublishAsync(object id);
}

public class MockBus : IBus
{
    public async Task PublishAsync(object id)
    {
        await Task.CompletedTask;
    }
}