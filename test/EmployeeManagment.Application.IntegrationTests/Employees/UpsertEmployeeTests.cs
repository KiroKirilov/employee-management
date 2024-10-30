using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Application.Employees;
using EmployeeManagement.Domain.Abstractions;
using EmployeeManagment.Application.IntegrationTests.Infrastructure;
using Xunit;

namespace EmployeeManagment.Application.IntegrationTests.Employees;
public class UpsertEmployeeTests : BaseIntegrationTest
{
    public UpsertEmployeeTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpsertEmployee_ReturnsFailure_WhenCommandDoesntPassValidation()
    {
        // Arrange
        var command = new UpsertEmployeeCommand(0, "", "", 0);

        var expectedErrors = new List<Error>
        {
            new ("EmployeeId", "Employee cannot be their own manager"),
            new ("FullName", "'Full Name' must not be empty."),
            new ("Title", "'Title' must not be empty.")
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(expectedErrors);
    }

    [Fact]
    public async Task UpsertEmployee_CreatesEmployee_WhenIdIsDefault()
    {
        // Arrange
        var command = new UpsertEmployeeCommand(0, "John Doe_Test", "Developer", null);

        // Act
        var result = await Sender.Send(command);

        // Assert
        var employees = await EmployeeRepository.GetAll();
        result.IsSuccess.Should().BeTrue();
        employees.Should().ContainSingle(e => e.FullName == "John Doe_Test");
    }

    [Fact]
    public async Task UpsertEmployee_UpdatesEmployee_WhenIdIsInDb()
    {
        // Arrange
        var command = new UpsertEmployeeCommand(34, "John Doe_Test_Edit", "Developer", null);

        // Act
        var result = await Sender.Send(command);

        // Assert
        var employees = await EmployeeRepository.GetAll();
        result.IsSuccess.Should().BeTrue();
        employees.Should().ContainSingle(e => e.FullName == "John Doe_Test");
        employees.Should().NotContain(e => e.FullName == "Item To Be Edited");
    }
}
