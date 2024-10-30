using EmployeeManagement.Application.Employees;
using EmployeeManagement.Domain.Employees;
using EmployeeManagment.Application.IntegrationTests.Infrastructure;

namespace EmployeeManagment.Application.IntegrationTests.Employees;
public class GetEmployeeByIdTests : BaseIntegrationTest
{
    private static readonly EmployeeDto ExpectedEmployee = new()
    {
        EmployeeId = 33,
        FullName = "string",
        Title = "string",
        ManagerId = 40,
        ManagedEmployees = [
            new() {
                EmployeeId= 35,
                FullName= "string",
                Title= "string",
                ManagerId= 33,
                ManagedEmployees= [
                    new() {
                        EmployeeId= 36,
                        FullName= "string",
                        Title= "string",
                        ManagerId= 35,
                        ManagedEmployees= [
                            new() {
                                EmployeeId= 37,
                                FullName= "string",
                                Title= "string",
                                ManagerId= 36,
                                ManagedEmployees= [
                                    new() {
                                        EmployeeId= 38,
                                        FullName= "string",
                                        Title= "string",
                                        ManagerId= 37,
                                        ManagedEmployees= []
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }
        ]
    };

    public GetEmployeeByIdTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetEmployeeById_ShouldReturnEmployeeAsTree()
    {
        // Arrange
        var command = new GetEmployeeByIdQuery(33);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(ExpectedEmployee);
    }

    [Fact]
    public async Task GetEmployeeById_ShouldReturnFailure_WhenEmployeeDoesntExist()
    {
        // Arrange
        var command = new GetEmployeeByIdQuery(999);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(EmployeeErrors.NotFound);
    }
}
