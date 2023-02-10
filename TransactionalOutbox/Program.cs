using Microsoft.EntityFrameworkCore;
using TransactionalOutbox.BackgroundServices;
using TransactionalOutbox.Model;
using TransactionalOutbox.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NotifDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("NotificationDb"));
});

if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddHostedService<NotifSenderBackgroundService>();
}

builder.Services.AddScoped<IOutBox, SQLServerOutBox>();
builder.Services.AddSingleton<IBus, MockBus>();

var app = builder.Build();


app.MapGet("/", (NotifDbContext context) => context.Notifications.FirstOrDefault()?.CreatedAt);

app.Run();


