using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Application.Employees;
using EmployeeManagement.Domain.Employees;
using System.Data;

namespace EmployeeManagment.Application.UnitTests.Employees;
public class GetEmployeeByIdTests
{
    private readonly IEmployeeRepository _employeeRepository;

    private readonly GetEmployeeByIdQueryHandler _handler;

    public GetEmployeeByIdTests()
    {
        _employeeRepository = Substitute.For<IEmployeeRepository>();
        _handler = new GetEmployeeByIdQueryHandler(_employeeRepository);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Handle_ShouldReturnFailure_WhenIdIsLessThanOrEqualToZero(int id)
    {
        // Arrange
        var query = new GetEmployeeByIdQuery(id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(EmployeeErrors.NotFound);
    }


    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenNoDataIsFound()
    {
        // Arrange
        var query = new GetEmployeeByIdQuery(1);
        _employeeRepository.GetEmployeeWithManaged(1, Arg.Any<IDbConnection>(), Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain(EmployeeErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmployeeAsTree()
    {
        // Arrange
        var employeeId = 1;
        var employees = new List<EmployeeDto>
        {
            new() { EmployeeId = 1, FullName = "John Doe" },
            new() { EmployeeId = 2, FullName = "Jane Doe", ManagerId = 1 },
            new() { EmployeeId = 3, FullName = "John Smith", ManagerId = 1 },
            new() { EmployeeId = 4, FullName = "John Smith2", ManagerId = 3 },
            new() { EmployeeId = 5, FullName = "John Smith3", ManagerId = 3 },
            new() { EmployeeId = 6, FullName = "John Smith4", ManagerId = 2 },
        };

        var expectedEmployee = new EmployeeDto
            {
                EmployeeId = 1,
                FullName = "John Doe",
                ManagedEmployees = [
                    new() { 
                        EmployeeId = 2, 
                        FullName = "Jane Doe",
                        ManagerId = 1,
                        ManagedEmployees = [
                            new() { EmployeeId = 6, FullName = "John Smith4", ManagerId = 2 }
                        ]
                    },
                    new() {
                        EmployeeId = 3,
                        FullName = "John Smith",
                        ManagerId = 1,
                        ManagedEmployees = [
                            new() { EmployeeId = 4, FullName = "John Smith2", ManagerId = 3 },
                            new() { EmployeeId = 5, FullName = "John Smith3", ManagerId = 3 }
                        ]
                    }
                ]
            };

        _employeeRepository.GetEmployeeWithManaged(employeeId, Arg.Any<IDbConnection>(), Arg.Any<CancellationToken>())
            .Returns(employees);

        // Act
        var result = await _handler.Handle(new GetEmployeeByIdQuery(1), default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedEmployee);
    }
}
