using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Domain.Abstractions;
using EmployeeManagement.Domain.Employees;
using MediatR;

namespace EmployeeManagement.Application.Employees;

public sealed record DeleteEmployeeCommand(int Id): IRequest<Result>;

internal sealed class DeleteEmployeeCommandHandler (IEmployeeRepository employeeRepository) : IRequestHandler<DeleteEmployeeCommand, Result>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;

    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        if (request.Id <= 0)
        {
            return Result.Failure(EmployeeErrors.NotFound);
        }

        var affectedRows = await _employeeRepository.Delete(request.Id, cancellationToken: ct);

        if (affectedRows == 0)
        {
            return Result.Failure(EmployeeErrors.NotFound);
        }

        return Result.Success();
    }
}
