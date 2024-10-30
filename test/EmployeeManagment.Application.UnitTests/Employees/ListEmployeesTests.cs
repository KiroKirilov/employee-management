using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Application.Employees;

namespace EmployeeManagment.Application.UnitTests.Employees;
public class ListEmployeesTests
{
    private readonly IEmployeeRepository _employeeRepository;

    private readonly ListEmployeesQueryHandler _handler;

    public ListEmployeesTests()
    {
        _employeeRepository = Substitute.For<IEmployeeRepository>();
        _handler = new ListEmployeesQueryHandler(_employeeRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfEmployeesAsTree()
    {
        // Arrange
        var employees = new List<EmployeeDto>
        {
            new() { EmployeeId = 1, FullName = "John Doe" },
            new() { EmployeeId = 2, FullName = "Jane Doe" },
            new() { EmployeeId = 3, FullName = "John Smith", ManagerId = 1 },
            new() { EmployeeId = 4, FullName = "John Smith2", ManagerId = 3 },
            new() { EmployeeId = 5, FullName = "John Smith3", ManagerId = 3 }
        };

        var expectedEmployees = new List<EmployeeDto>
        {
            new()
            {
                EmployeeId = 1,
                FullName = "John Doe",
                ManagedEmployees = [
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
            },
            new() { EmployeeId = 2, FullName = "Jane Doe" },
        };

        _employeeRepository.GetAll(default).ReturnsForAnyArgs(employees);

        // Act
        var result = await _handler.Handle(new ListEmployeesQuery(), default);

        // Assert
        result.Should().BeEquivalentTo(expectedEmployees);
    }
}
