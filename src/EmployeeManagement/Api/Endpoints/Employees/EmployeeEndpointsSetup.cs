namespace EmployeeManagement.Api.Endpoints.Employees;

public static class EmployeeEndpointsSetup
{
    private const string TagName = "Employee";
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("employees").WithTags(TagName);

        group.MapGet("{id:int}", GetById.Handle).WithName(nameof(GetById));

        return group;
    }
}
