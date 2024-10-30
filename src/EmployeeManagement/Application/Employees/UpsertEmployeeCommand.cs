using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Domain.Abstractions;
using EmployeeManagement.Domain.Employees;
using FluentValidation;
using MediatR;

namespace EmployeeManagement.Application.Employees;

public sealed record UpsertEmployeeCommand(
    int EmployeeId,
    string FullName,
    string Title,
    int? ManagerId) : IRequest<Result>;


internal sealed class UpsertEmployeeCommandValidator : AbstractValidator<UpsertEmployeeCommand>
{
    public UpsertEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThanOrEqualTo(0)
            .Must((employee, id) => id != employee.ManagerId).WithMessage("Employee cannot be their own manager");

        RuleFor(x => x.FullName).NotNull().NotEmpty();
        RuleFor(x => x.Title).NotNull().NotEmpty();
    }
}


internal sealed class UpsertEmployeeCommandHandler(IEmployeeRepository employeeRepository, ISqlConnectionFactory sqlConnectionFactory) : IRequestHandler<UpsertEmployeeCommand, Result>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<Result> Handle(UpsertEmployeeCommand request, CancellationToken ct)
    {
        using var connection = _sqlConnectionFactory.GetOpenConnection();
        var managerId = request.ManagerId <= 0 ? null : request.ManagerId;

        if (managerId is not null && 
            !await _employeeRepository.Exists(managerId.Value, connection, ct))
        {
            return Result.Failure(EmployeeErrors.NotFound);
        }

        var employee = new Employee(
            request.EmployeeId,
            request.FullName,
            request.Title,
            managerId);

        var affectedRows = 0;
        if (employee.EmployeeId == default)
        {
            affectedRows = await _employeeRepository.Insert(employee, connection, ct);
        }
        else
        {
            var updateWillCauseCircularDependency = await _employeeRepository.WillUpdateCauseCircularDependency(employee, connection, ct);

            if (updateWillCauseCircularDependency)
            {
                return Result.Failure(EmployeeErrors.CircularDependency);
            }

            affectedRows = await _employeeRepository.Update(employee, connection, ct);
        }

        if (affectedRows == 0)
        {
            return Result.Failure(EmployeeErrors.UnsuccessfulUpsert);
        }

        return Result.Success();
    }
}
