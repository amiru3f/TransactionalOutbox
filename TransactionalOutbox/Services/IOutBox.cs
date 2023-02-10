namespace TransactionalOutbox.Services;

public interface IOutBox
{
    Task<int> ExecuteSend(int batchSize, CancellationToken cancellationToken);
}