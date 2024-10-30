namespace EmployeeManagement.Domain.Abstractions;

public record Error(string Code, string Name)
{
    public static Error None => new("none", "None");
}
