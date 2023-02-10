using Microsoft.EntityFrameworkCore;
using TransactionalOutbox.Model;

namespace TransactionalOutbox.BackgroundServices;

public class ImmigratorBackgroundService : BackgroundService
{
    private readonly TaskCompletionSource _source = new();
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IHostApplicationLifetime lifetime;

    public ImmigratorBackgroundService(IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime lifetime)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotifDbContext>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await dbContext.Database.MigrateAsync();
                break;
            }
            catch
            {
                await Task.Delay(1000);
                continue;
            }
        }
    }
}