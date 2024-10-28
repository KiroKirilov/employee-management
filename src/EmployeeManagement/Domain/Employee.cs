namespace EmployeeManagement.Domain;

public class Employee
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public List<Employee> ManagedEmployees { get; set; } = [];
}
