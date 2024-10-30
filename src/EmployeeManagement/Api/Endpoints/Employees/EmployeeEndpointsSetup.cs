namespace EmployeeManagement.Api.Endpoints.Employees;

public static class EmployeeEndpointsSetup
{
    private const string TagName = "Employee";
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("employees").WithTags(TagName);

        group.MapGet("", List.Handle).WithName(nameof(List));
        group.MapGet("{id:int}", GetById.Handle).WithName(nameof(GetById));
        group.MapPut("", Upsert.Handle).WithName(nameof(Upsert));
        group.MapDelete("{id:int}", Delete.Handle).WithName(nameof(Delete));

        return group;
    }
}
