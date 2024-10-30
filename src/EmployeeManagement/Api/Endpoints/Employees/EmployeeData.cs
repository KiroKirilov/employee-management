namespace EmployeeManagement.Api.Endpoints.Employees;

public sealed record EmployeeData(
    int EmployeeId,
    string FullName,
    string Title,
    int? ManagerId);