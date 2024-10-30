namespace EmployeeManagement.Application.Employees;

public class EmployeeDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int? ManagerId { get; set; }
    public List<EmployeeDto> ManagedEmployees { get; set; } = [];
}
