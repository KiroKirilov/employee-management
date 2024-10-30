using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace EmployeeManagment.Application.IntegrationTests.Infrastructure;
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("employee-management")
        .WithUsername("employee-management")
        .WithPassword("employee-management")
        .WithStartupCallback(async (container, ct) =>
        {
            var dllScript = await File.ReadAllTextAsync("Infrastructure/Data/employee-schema.sql", ct);
            var employeesDataScript = await File.ReadAllTextAsync("Infrastructure/Data/employee-data.sql", ct);

            await container.ExecScriptAsync(dllScript);
            await container.ExecScriptAsync(employeesDataScript);
        })
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ISqlConnectionFactory>();

            services.AddSingleton<ISqlConnectionFactory>(_ =>
                new SqlConnectionFactory(_dbContainer.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
