namespace EmployeeManagement.Domain.Employees;

public class Employee
{
    public Employee(int employeeId, string fullName, string title, int? managerId)
    {
        EmployeeId = employeeId;
        FullName = fullName;
        Title = title;
        ManagerId = managerId;
    }

    public int EmployeeId { get; set; }
    public string FullName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public List<Employee> ManagedEmployees { get; set; } = [];
}
