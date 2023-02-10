using TransactionalOutbox.Services;

namespace TransactionalOutbox.BackgroundServices;

public class NotifSenderBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<NotifSenderBackgroundService> logger;

    public NotifSenderBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<NotifSenderBackgroundService> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var outbox = scope.ServiceProvider.GetRequiredService<IOutBox>();

            try
            {
                int sentCount = await outbox.ExecuteSend(10, stoppingToken);
                logger.LogInformation("count of sent: {sent}", sentCount);
            }
            catch (System.Exception ex)
            {
                logger.LogCritical(ex, ex.Message);
            }
        }
    }
}