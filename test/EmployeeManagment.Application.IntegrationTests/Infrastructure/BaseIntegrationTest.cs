using EmployeeManagement.Application.Abstractions.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagment.Application.IntegrationTests.Infrastructure;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IServiceScope _scope;
    protected readonly ISender Sender;
    protected readonly IEmployeeRepository EmployeeRepository;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        EmployeeRepository = _scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
    }
}
