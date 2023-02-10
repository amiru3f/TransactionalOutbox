using TransactionalOutbox.Model;

namespace TransactionalOutbox.Services;

public class SQLServerOutBox : IOutBox
{
    protected readonly NotifDbContext context;
    private readonly IBus bus;

    public SQLServerOutBox(NotifDbContext notifDbContext, IBus bus)
    {
        this.context = notifDbContext;
        this.bus = bus;
    }

    public async Task<int> ExecuteSend(int batchSize, CancellationToken cancellationToken)
    {
        using var trx = await context.Database.BeginTransactionAsync();

        try
        {
            var notifications = await context.TopUnsentNotifications(batchSize);

            if (notifications == null || notifications.Count == 0) return 0;

            var notificationIds = notifications.Select(x => x.Id).ToList();
            int affectedRows = await context.MarkNotificationsAsSent(notificationIds);

            await bus.PublishAsync("");
            await trx.CommitAsync();

            return notifications.Count;
        }
        catch
        {
            await trx.RollbackAsync();
            return 0;
        }
    }
}