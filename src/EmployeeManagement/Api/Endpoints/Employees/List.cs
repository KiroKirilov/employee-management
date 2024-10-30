using EmployeeManagement.Application.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Endpoints.Employees;

public static class List
{
    public static async Task<IResult> Handle([FromServices] ISender sender, CancellationToken ct)
    {
        var result = await sender.Send(new ListEmployeesQuery(), ct);

        return Results.Ok(result);
    }
}
