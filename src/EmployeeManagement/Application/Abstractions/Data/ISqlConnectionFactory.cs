using System.Data;

namespace EmployeeManagement.Application.Abstractions.Data;

public interface ISqlConnectionFactory
{
    IDbConnection GetOpenConnection();
}
