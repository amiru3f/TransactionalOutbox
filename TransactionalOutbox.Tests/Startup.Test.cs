global using Xunit;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TransactionalOutbox;
using TransactionalOutbox.Model;
using TransactionalOutbox.Services;

namespace TransactionalOutbox.Tests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<NotifDbContext>((container, options) =>
            {
                options.UseSqlServer("server=localhost;database=test;User Id=sa;Password=123456aA;trust server certificate=true;Max Pool Size=50000;Pooling=True;");
            });

            services.AddScoped<IOutBox, SQLServerOutBoxExtend>();


        });

        builder.UseEnvironment("Test");
    }
}