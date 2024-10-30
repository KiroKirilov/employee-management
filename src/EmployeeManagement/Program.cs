using EmployeeManagement.Api.Endpoints.Employees;
using EmployeeManagement.Api.Extensions;
using EmployeeManagement.Application;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Presentation;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddPresentation()
    .AddApplication()
    .AddInfrastructure(configuration);

var app = builder.Build();

app.CreateDbSchema();

if (app.Environment.IsDevelopment())
{
    app.SeedData();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseInvalidJsonHandling();
app.UseCustomExceptionHandling();

var apiGroup = app.MapGroup("api");
apiGroup.MapEmployeeEndpoints();

app.Run();

// Needed for tests
public partial class Program;