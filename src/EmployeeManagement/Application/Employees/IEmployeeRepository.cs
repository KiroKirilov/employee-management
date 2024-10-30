using EmployeeManagement.Application.Employees;
using EmployeeManagement.Domain.Employees;
using System.Data;

namespace EmployeeManagement.Application.Abstractions.Data;

public interface IEmployeeRepository
{
    Task<List<EmployeeDto>> GetAll(IDbConnection? existingConnection = null, CancellationToken cancellationToken = default);
    Task<List<EmployeeDto>> GetEmployeeWithManaged(int employeeId, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default);
    Task<int> Insert(Employee employee, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default);
    Task<int> Update(Employee employee, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default);
    Task<bool> Exists(int employeeId, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default);
    Task<int> Delete(int employeeId, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default);
    Task<bool> WillUpdateCauseCircularDependency(Employee employee, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default);
}
