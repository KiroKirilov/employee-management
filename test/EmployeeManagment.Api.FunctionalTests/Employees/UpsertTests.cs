using EmployeeManagement.Api.Endpoints.Employees;
using EmployeeManagment.Api.FunctionalTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace EmployeeManagment.Api.FunctionalTests.Employees;
public class UpsertTests : BaseFunctionalTest
{
    public UpsertTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Theory]
    [InlineData(0, "", "")]
    [InlineData(-3, "Test", "Test")]
    [InlineData(1, "Test", "Test", 1)]
    public async Task Upsert_ShouldReturnBadRequest_WhenRequestHasInvalidData(
        int employeeId,
        string fullName,
        string title,
        int? managerId = null)
    {
        // Arrange
        var request = new EmployeeData(employeeId, fullName, title, managerId);

        // Act
        var response = await HttpClient.PutAsJsonAsync("/api/employees", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Upsert_ShouldReturnBadRequest_WhenRequestHasInvalidStructure()
    {
        // Arrange
        var content = new StringContent("{123}", Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PutAsync("/api/employees", content);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseString.Should().Contain("The JSON payload provided is invalid");
    }
}
