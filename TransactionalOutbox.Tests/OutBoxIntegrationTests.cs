using Microsoft.Extensions.DependencyInjection;
using TransactionalOutbox;
using TransactionalOutbox.Services;

namespace TransactionalOutbox.Tests;

public class OutBoxIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> sut;

    public OutBoxIntegrationTests(CustomWebApplicationFactory<Program> app)
    {
        sut = app;
    }
    [Fact]
    public async Task GivenLotsOfOutBoxItems_AssertConcurrency()
    {

        int countOfDays = 5;

        int countOfMessages = 5000;
        int batchSize = 10;
        int concurrencyLevel = 5000;

        await using var sutScope = sut.Services.CreateAsyncScope();
        var testOutBox = sutScope.ServiceProvider.GetRequiredService<IOutBox>() as SQLServerOutBoxExtend;

        try
        {
            //setup
            await testOutBox!.PurgeNotifications();
            await testOutBox!.AddMockNotifications(countOfDays, countOfMessages);
            List<Task<int>> sendTaskList = new List<Task<int>>();
            while (concurrencyLevel-- > 0) sendTaskList.Add(TrySend());

            //assert
            var affectedRows = (await Task.WhenAll(sendTaskList)).Sum();
            Assert.Equal(countOfMessages, affectedRows);
        }
        finally
        {
            //clean-up
            await testOutBox!.PurgeNotifications();
        }

        async Task<int> TrySend()
        {
            await using var scope = sut.Services.CreateAsyncScope();
            var outbox = scope.ServiceProvider.GetRequiredService<IOutBox>();
            return await outbox.ExecuteSend(batchSize, CancellationToken.None);
        }
    }
}