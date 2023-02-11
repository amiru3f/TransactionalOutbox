using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionalOutbox.Model;
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
    public async Task GivenOnlyOneOutboxMessage_MultipleSendExecutionsShouldReturnZeroAffectedRows()
    {

        await using var sutScope = sut.Services.CreateAsyncScope();
        var testOutBox = sutScope.ServiceProvider.GetRequiredService<IOutBox>() as SQLServerOutBoxExtend;

        try
        {
            //setup
            InitTestScope(sutScope, nameof(GivenOnlyOneOutboxMessage_MultipleSendExecutionsShouldReturnZeroAffectedRows));
            await testOutBox!.PurgeNotifications();
            await testOutBox.AddMockNotifications(1, 1);


            //execute
            var rowsAffected = await testOutBox.ExecuteSend(5, CancellationToken.None);
            var rowsAffectedAgain = await testOutBox.ExecuteSend(5, CancellationToken.None);

            //assert
            Assert.Equal(1, rowsAffected);
            Assert.Equal(0, rowsAffectedAgain);
        }
        finally
        {
            //cleanup
            await testOutBox!.PurgeNotifications();
        }
    }

    [Fact]
    public async Task GivenLotsOfOutBoxItems_AssertConcurrency()
    {
        int countOfDays = 5;

        int countOfMessages = 5000;
        int batchSize = 10;
        int concurrencyLevel = 500;

        await using var sutScope = sut.Services.CreateAsyncScope();
        var testOutBox = sutScope.ServiceProvider.GetRequiredService<IOutBox>() as SQLServerOutBoxExtend;

        try
        {
            //setup
            InitTestScope(sutScope, nameof(GivenLotsOfOutBoxItems_AssertConcurrency));

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

            InitTestScope(scope, nameof(GivenLotsOfOutBoxItems_AssertConcurrency), migrate: false);
            var outbox = scope.ServiceProvider.GetRequiredService<IOutBox>();
            return await outbox.ExecuteSend(batchSize, CancellationToken.None);
        }
    }


    private void InitTestScope(IServiceScope scope, string connectionStringName, bool migrate = true)
    {
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = config.GetConnectionString(connectionStringName);
        var db = scope.ServiceProvider.GetRequiredService<NotifDbContext>();

        db.Database.SetConnectionString(connectionString);

        if (migrate)
        {
            db.Database.Migrate();
        }
    }
}