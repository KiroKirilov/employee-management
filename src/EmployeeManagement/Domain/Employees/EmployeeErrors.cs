using EmployeeManagement.Domain.Abstractions;

namespace EmployeeManagement.Domain.Employees;

public static class EmployeeErrors
{
    public const string NotFoundCode = "Employee.NotFound";
    public static Error NotFound = new(NotFoundCode, "Employee not found");

    public static Error UnsuccessfulUpsert = new("Employee.UnsuccessfulUpsert", "Coulnd't create/update employee");
    public static Error CircularDependency = new("Employee.CircularDependency", "This update will cause a circular dependency");
}
