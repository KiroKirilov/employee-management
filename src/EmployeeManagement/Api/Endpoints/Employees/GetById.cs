using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Endpoints.Employees;
public class GetById
{
    public static async Task<IResult> Handle([FromServices] ISender sender, int id)
    {
        return Results.Ok($"henlo, {id}");
    }
}
