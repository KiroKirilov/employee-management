
using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Application.Employees;
using EmployeeManagement.Domain.Employees;
using System.Data;

namespace EmployeeManagment.Application.UnitTests.Employees;
public class UpsertEmployeeTests
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISqlConnectionFactory _connectionFactory;

    private readonly UpsertEmployeeCommandHandler _handler;

    public UpsertEmployeeTests()
    {
        _employeeRepository = Substitute.For<IEmployeeRepository>();
        _connectionFactory = Substitute.For<ISqlConnectionFactory>();

        _handler = new UpsertEmployeeCommandHandler(_employeeRepository, _connectionFactory);
    }


    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenManagerDoesntExist()
    {
        // Arrange
        var command = new UpsertEmployeeCommand(1, "John Doe", "Manager", 2);
        _employeeRepository.Exists(2, Arg.Any<IDbConnection>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(EmployeeErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoRowsWereAffectedOnInsert()
    {
        // Arrange
        var command = new UpsertEmployeeCommand(0, "John Doe", "Manager", null);
        _employeeRepository.Insert(default!).ReturnsForAnyArgs(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(EmployeeErrors.UnsuccessfulUpsert);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRowsWereAffectedOnInsert()
    {
        // Arrange
        var command = new UpsertEmployeeCommand(0, "John Doe", "Manager", null);
        _employeeRepository.Insert(default!).ReturnsForAnyArgs(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUpdateWillCauseCircularDependency()
    {
        // Arrange
        var command = new UpsertEmployeeCommand(1, "John Doe", "Manager", null);
        _employeeRepository.WillUpdateCauseCircularDependency(default!).ReturnsForAnyArgs(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(EmployeeErrors.CircularDependency);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenWhenNoRowsWereAffectedOnUpdate()
    {
        // Arrange
        var command = new UpsertEmployeeCommand(1, "John Doe", "Manager", null);
        _employeeRepository.WillUpdateCauseCircularDependency(default!).ReturnsForAnyArgs(false);

        _employeeRepository.Update(default!).ReturnsForAnyArgs(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(EmployeeErrors.UnsuccessfulUpsert);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenWhenRowsWereAffectedOnUpdate()
    {
        // Arrange
        var command = new UpsertEmployeeCommand(1, "John Doe", "Manager", null);
        _employeeRepository.WillUpdateCauseCircularDependency(default!).ReturnsForAnyArgs(false);

        _employeeRepository.Update(default!).ReturnsForAnyArgs(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
