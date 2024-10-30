using Dapper;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Application.Abstractions.Data;
using System.Data;

namespace EmployeeManagement.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseCustomExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void UseInvalidJsonHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<InvalidJsonHandlingMiddleware>();
    }

    public static void CreateDbSchema(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        using var connection = sqlConnectionFactory.GetOpenConnection();

        var employeesTableExists = connection.ExecuteScalar<bool>(@"
            SELECT EXISTS (
               SELECT FROM information_schema.tables 
               WHERE  table_schema = 'public'
               AND    table_name   = 'employees'
               );");

        if (employeesTableExists)
        {
            return;
        }

        var ddlScript = File.ReadAllText("Infrastructure/Data/employee-schema.sql");
        connection.Execute(ddlScript);
    }

    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        using var connection = sqlConnectionFactory.GetOpenConnection();

        var employeesCount = connection.ExecuteScalar<int>(@"SELECT count(*) FROM public.employees");

        if (employeesCount > 0)
        {
            return;
        }

        var dataScript = File.ReadAllText("Infrastructure/Data/employee-data.sql");
        connection.Execute(dataScript);
    }
}
