using Dapper;
using EmployeeManagement.Application.Abstractions.Data;
using EmployeeManagement.Application.Employees;
using EmployeeManagement.Domain.Employees;
using System.Data;

namespace EmployeeManagement.Infrastructure.Employees;

public class EmployeeRepository(ISqlConnectionFactory sqlConnectionFactory) : IEmployeeRepository
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<List<EmployeeDto>> GetAll(IDbConnection? existingConnection = null, CancellationToken cancellationToken = default)
    {
        var connection = existingConnection ?? _sqlConnectionFactory.GetOpenConnection();
        try
        {
            var commandDefinition = new CommandDefinition(@"
                SELECT 
                    employee_id as EmployeeId, 
                    full_name as FullName, 
                    title as Title, 
                    manager_id as ManagerId
                FROM public.employees;", cancellationToken: cancellationToken);

            return (await connection.QueryAsync<EmployeeDto>(commandDefinition)).ToList();
        }
        finally
        {
            if (existingConnection is null)
            {
                connection.Dispose();
            }
        }
    }

    public async Task<int> Delete(int employeeId, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default)
    {
        var connection = existingConnection ?? _sqlConnectionFactory.GetOpenConnection();
        try
        {
            var commandDefinition = new CommandDefinition(@"
                DELETE FROM employees 
                WHERE employee_id = @employeeId;", new { employeeId }, cancellationToken: cancellationToken);

            return await connection.ExecuteAsync(commandDefinition);
        }
        finally
        {
            if (existingConnection is null)
            {
                connection.Dispose();
            }
        }
    }

    public async Task<bool> Exists(int employeeId, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default)
    {
        var connection = existingConnection ?? _sqlConnectionFactory.GetOpenConnection();
        try
        {
            var commandDefinition = new CommandDefinition(@"
                SELECT EXISTS (
                    SELECT 1
                    FROM employees
                    WHERE employee_id = @employeeId
                );", new { employeeId }, cancellationToken: cancellationToken);

            return await connection.ExecuteScalarAsync<bool>(commandDefinition);
        }
        finally
        {
            if (existingConnection is null)
            {
                connection.Dispose();
            }
        }

    }

    public async Task<int> Update(Employee employee, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default)
    {
        var connection = existingConnection ?? _sqlConnectionFactory.GetOpenConnection();
        try
        {
            var updateCommandDefinition = new CommandDefinition(@"
                UPDATE employees 
                SET full_name = @FullName, title = @Title, manager_id = @ManagerId 
                WHERE employee_id = @EmployeeId;", employee, cancellationToken: cancellationToken);

            return await connection.ExecuteAsync(updateCommandDefinition);
        }
        finally
        {
            if (existingConnection is null)
            {
                connection.Dispose();
            }
        }
    }

    public async Task<int> Insert(Employee employee, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default)
    {
        var connection = existingConnection ?? _sqlConnectionFactory.GetOpenConnection();
        try
        {
            var insertCommandDefinition = new CommandDefinition(@"
                    INSERT INTO employees (full_name, title, manager_id)
                    VALUES (@FullName, @Title, @ManagerId);", employee, cancellationToken: cancellationToken);

            return await connection.ExecuteAsync(insertCommandDefinition);
        }
        finally
        {
            if (existingConnection is null)
            {
                connection.Dispose();
            }
        }
    }

    public async Task<bool> WillUpdateCauseCircularDependency(Employee employee, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default)
    {
        var connection = existingConnection ?? _sqlConnectionFactory.GetOpenConnection();
        try
        {
            // Traverses the hierarchy *upwards* to check if the employee update will make it so it is its own manager indirectly
            var commandDefinition = new CommandDefinition(@"
                    WITH RECURSIVE managers AS (
                      SELECT
                        employee_id,
                        manager_id,
                        full_name,
                        title
                      FROM
                        employees
                      WHERE
                        employee_id = @ManagerId
                      UNION
                      SELECT
                        e.employee_id,
                        e.manager_id,
                        e.full_name,
                        e.title
                      FROM
                        employees e
                        INNER JOIN managers s ON s.manager_id = e.employee_id
                    )
                    SELECT EXISTS (
                        SELECT 1 FROM managers
	                    where employee_id = @EmployeeId
                    );", new { employee.EmployeeId, employee.ManagerId }, cancellationToken: cancellationToken);

            return await connection.ExecuteScalarAsync<bool>(commandDefinition);
        }
        finally
        {
            if (existingConnection is null)
            {
                connection.Dispose();
            }
        }
    }

    public async Task<List<EmployeeDto>> GetEmployeeWithManaged(int employeeId, IDbConnection? existingConnection = null, CancellationToken cancellationToken = default)
    {
        var connection = existingConnection ?? _sqlConnectionFactory.GetOpenConnection();
        try
        {
            // Traverses the hierarchy *downwards* get the employee and all employees managed by them directly or indirectly
            var commandDefinition = new CommandDefinition(@"
                    WITH RECURSIVE managed AS (
                      SELECT
                        employee_id,
                        manager_id,
                        full_name,
                        title
                      FROM
                        employees
                      WHERE
                        employee_id = @employeeId
                      UNION
                      SELECT
                        e.employee_id,
                        e.manager_id,
                        e.full_name,
                        e.title
                      FROM
                        employees e
                        INNER JOIN managed s ON s.employee_id = e.manager_id
                    )
                    SELECT 
                        employee_id as EmployeeId, 
                        full_name as FullName, 
                        title as Title, 
                        manager_id as ManagerId
                    FROM managed;", new { employeeId }, cancellationToken: cancellationToken);

            return (await connection.QueryAsync<EmployeeDto>(commandDefinition)).ToList();
        }
        finally
        {
            if (existingConnection is null)
            {
                connection.Dispose();
            }
        }
    }
}
