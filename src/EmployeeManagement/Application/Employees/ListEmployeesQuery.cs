using EmployeeManagement.Application.Abstractions.Data;
using MediatR;

namespace EmployeeManagement.Application.Employees;

public sealed record ListEmployeesQuery(): IRequest<List<EmployeeDto>>;

internal sealed class ListEmployeesQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<ListEmployeesQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<List<EmployeeDto>> Handle(ListEmployeesQuery request, CancellationToken ct)
    {
        var employees = await _employeeRepository.GetAll(cancellationToken: ct);

        return BuildCompanyTree(employees);
    }

    private static List<EmployeeDto> BuildCompanyTree(List<EmployeeDto> employees)
    {
        var employeeDictionary = employees.ToDictionary(e => e.EmployeeId);
        var rootEmployees = new List<EmployeeDto>();

        foreach (var employee in employees)
        {
            if (employee.ManagerId is null)
            {
                rootEmployees.Add(employee);
            }
            else
            {
                if (employeeDictionary.TryGetValue(employee.ManagerId.Value, out var manager))
                {
                    manager.ManagedEmployees ??= [];

                    manager.ManagedEmployees.Add(employee);
                }
            }
        }

        return rootEmployees;
    }
}
