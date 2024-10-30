using EmployeeManagement.Api.Middleware;

namespace EmployeeManagement.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseCustomExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
    public static void UseInvalidJsonHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<InvalidJsonHandlingMiddleware>();
    }
}
