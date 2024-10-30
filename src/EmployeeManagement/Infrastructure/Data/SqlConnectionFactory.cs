using EmployeeManagement.Application.Abstractions.Data;
using Npgsql;
using System.Data;

namespace EmployeeManagement.Infrastructure.Data;

public class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    private readonly string _connectionString = connectionString;

    public IDbConnection GetOpenConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
