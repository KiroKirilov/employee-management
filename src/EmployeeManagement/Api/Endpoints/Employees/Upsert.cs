using EmployeeManagement.Application.Employees;
using EmployeeManagement.Domain.Employees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Endpoints.Employees;

public static class Upsert
{
    public static async Task<IResult> Handle([FromServices] ISender sender, EmployeeData data, CancellationToken ct)
    {
        var result = await sender.Send(new UpsertEmployeeCommand(
            data.EmployeeId,
            data.FullName,
            data.Title,
            data.ManagerId), ct);
        
        if (result.IsFailure)
        {
            return result.Errors[0].Code switch
            {
                EmployeeErrors.NotFoundCode => Results.NotFound(),
                _ => Results.BadRequest(result.Errors)
            };
        }

        return Results.Ok();
    }
}
