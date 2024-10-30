using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EmployeeManagement.Api.Middleware;

public class InvalidJsonHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
		try
		{
			await _next(context);
		}
		catch (BadHttpRequestException e)
			when (e.InnerException is JsonException)
		{
			context.Response.StatusCode = StatusCodes.Status400BadRequest;
			await context.Response.WriteAsJsonAsync(new ProblemDetails
			{
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid JSON",
                Detail = $"The JSON payload provided is invalid. ({e.InnerException.Message})",
            });
		}
    }
}
