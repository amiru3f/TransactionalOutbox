using Microsoft.EntityFrameworkCore;
using TransactionalOutbox.BackgroundServices;
using TransactionalOutbox.Model;
using TransactionalOutbox.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NotifDbContext>(options =>
{
    options.UseSqlServer("server=localhost;database=test;User Id=sa;Password=123456aA;trust server certificate=true");
});

if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddHostedService<NotifSenderBackgroundService>();
}

builder.Services.AddScoped<IOutBox, SQLServerOutBox>();
builder.Services.AddSingleton<IBus, MockBus>();

var app = builder.Build();


app.MapGet("/", (NotifDbContext context) => context.Notifications.FirstOrDefault().CreatedAt);

app.Run();


