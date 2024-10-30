using EmployeeManagement.Application.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Endpoints.Employees;

public static class Delete
{
    public static async Task<IResult> Handle([FromServices] ISender sender, int id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteEmployeeCommand(id), ct);

        if (result.IsFailure)
        {
            return Results.NotFound();
        }

        return Results.Ok();
    }
}
