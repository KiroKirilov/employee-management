using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Application.Employees;
using EmployeeManagement.Domain.Employees;
using System.Data;

namespace EmployeeManagment.Application.UnitTests.Employees;
public class DeleteEmployeeTests
{
	private readonly IEmployeeRepository _employeeRepository;

    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeTests()
    {
        _employeeRepository = Substitute.For<IEmployeeRepository>();

		_handler = new DeleteEmployeeCommandHandler(_employeeRepository);
    }

    [Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public async Task Handle_ShouldReturnFailure_WhenIdIsLessThanOrEqualToZero(int id)
	{
		// Arrange
		var command = new DeleteEmployeeCommand(id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Errors.Should().Contain(EmployeeErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoRowsWereAffected()
    {
        // Arrange
        const int employeeId = 1;
        var command = new DeleteEmployeeCommand(employeeId);
        _employeeRepository.Delete(employeeId, Arg.Any<IDbConnection>(), Arg.Any<CancellationToken>()).Returns(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(EmployeeErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenRowWasAffected()
    {
        // Arrange
        const int employeeId = 1;
        var command = new DeleteEmployeeCommand(employeeId);
        _employeeRepository.Delete(employeeId, Arg.Any<IDbConnection>(), Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
