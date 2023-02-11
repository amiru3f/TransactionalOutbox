namespace TransactionalOutbox.Services;

public interface IBus
{
    Task PublishAsync(object id);
}

public class Bus : IBus
{
    public async Task PublishAsync(object id)
    {
        await Task.CompletedTask;
    }
}