using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Domain.Abstractions;
using EmployeeManagement.Domain.Employees;
using MediatR;

namespace EmployeeManagement.Application.Employees;

public sealed record GetEmployeeByIdQuery(int Id): IRequest<Result<EmployeeDto>>;

internal sealed class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository) : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken ct)
    {
        if (request.Id <= 0)
        {
            return Result.Failure<EmployeeDto>(EmployeeErrors.NotFound);
        }

        var employeeWithManaged = await _employeeRepository.GetEmployeeWithManaged(request.Id, cancellationToken: ct);

        if (employeeWithManaged.Count == 0)
        {
            return Result.Failure<EmployeeDto>(EmployeeErrors.NotFound);
        }

        var rootEmployee = employeeWithManaged[0];
        employeeWithManaged.RemoveAt(0);
        var employeeDictionary = employeeWithManaged.ToDictionary(x => x.EmployeeId);

        foreach (var employee in employeeWithManaged)
        {
            if (employee.ManagerId is null)
            {
                continue;
            }

            if (employee.ManagerId == rootEmployee.EmployeeId)
            {
                rootEmployee.ManagedEmployees ??= [];

                rootEmployee.ManagedEmployees.Add(employee);
            }
            else if (employeeDictionary.TryGetValue(employee.ManagerId.Value, out var manager))
            {
                manager.ManagedEmployees ??= [];

                manager.ManagedEmployees.Add(employee);
            }
        }

        return Result.Success(rootEmployee);
    }
}
