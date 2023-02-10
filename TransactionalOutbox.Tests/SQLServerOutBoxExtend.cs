using Microsoft.EntityFrameworkCore;
using TransactionalOutbox.Model;
using TransactionalOutbox.Services;

namespace TransactionalOutbox;

public sealed class SQLServerOutBoxExtend : SQLServerOutBox
{
    public SQLServerOutBoxExtend(NotifDbContext notifDbContext, IBus bus) : base(notifDbContext, bus)
    {
    }

    public async Task PurgeNotifications()
    {
        await context.Notifications.ExecuteDeleteAsync();
    }

    public async Task AddMockNotifications(int countOfDays, int countOfMessages)
    {
        var from = DateTime.Now.AddDays(-1 * countOfDays);
        int diff = countOfMessages - countOfDays;

        for (int i = 0; i < countOfMessages; i++)
        {
            int randomDay = Random.Shared.Next(0, countOfDays);
            var occuranceDate = from.AddDays(randomDay).Date;

            context.Notifications.Add(new Notif()
            {
                CreatedAt = occuranceDate,
                Sent = false
            });
        }

        await context.SaveChangesAsync();
    }
}