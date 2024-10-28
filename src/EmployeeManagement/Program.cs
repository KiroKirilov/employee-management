using EmployeeManagement.Api.Endpoints.Employees;
using EmployeeManagement.Application;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Presentation;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddPresentation()
    .AddApplication()
    .AddInfrastructure(configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

var apiGroup = app.MapGroup("api");
apiGroup.MapEmployeeEndpoints();

app.Run();
