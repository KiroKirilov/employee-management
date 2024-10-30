# Overview
This project represents a simple implementation of a RESTful API for an employee management system. The API is implemented using .NET 8 Minimal API and Dapper for data access. The API provides the following endpoints:
- GET /api/employees: Returns a list of all employees in a tree-like view, where the root objects in the collection are the employees with no manager and they each have a list of their direct managed employees, which in turn have a list of their own and so on.
- GET /api/employees/{id}: Returns the employee with the specified id and a tree view of their direct and indirect managed employees.
- PUT /api/employees: Upserts an employee. If the employee's id is 0, it is inserted, otherwise it is updated.
- DELETE /api/employees/{id}: Deletes the employee with the specified id. All their direct managed employees become root employees.

**This solution took ~8 hours to build, working on it in small batches since Monday.**

# Structure
All the source code is located in the `src` folder. The solution itself has only one source code project called `EmployeeManagement`. The code in this project is structured in a fashion similar to Clean Architecture, with the folders `Api`, `Application`, `Domain`, and `Infrastructure` representing the different layers of the application. CQRS (with the `MediatR` package) is used to separate the read and write operations. 

- The `Api` folder contains the Minimal API endpoints, as well any API level concerns like global error handling, swagger doc, etc.
- The `Application` folder contains the command and query handlers
- The `Domain` folder contains the domain entity and abstractions like `Error` and `Result`
- The `Infrastructure` folder contains the data access code.

# Running the API
The project is configured to run with `docker-compose` which sets up the API and a PostgreSQL database. It can be ran with the `docker compose up --detach` command or by running the project in Visual Studio with the Docker Compose profile selected. By default the DB structure is setup on startup. If running in development, some dummy data is also seeded. To setup the DB structure manually, the `Infrastructure/Data/employee-schema.sql` and  `Infrastructure/Data/employee-data.sql` can be run against the database. The `employee-schema.sql` file creates the necessary table and the `employee-data.sql` file inserts some sample data.

The API can also be ran on its own by running the project in Visual Studio with the `EmployeeManagement` profile selected or with `dotnet run`.

More information on the app settings that need to be setup can be found in the [App settings](#App-settings) section.

# Testing
The project has three test projects created and setup:
- `EmployeeManagment.Application.UnitTests` contains unit tests for the command and query handlers
- `EmployeeManagment.Application.IntegrationTests` contains integration tests for the CQRS handlers, using the full `MediatR` pipeline and a real database, using is scaffolded using `Testcontainers`.
- `EmployeeManagment.Api.FunctionalTests` contains functional tests for the API endpoints, using `WebApplicationFactory` to host the API in memory and send requests to it.

# App settings
The app settings are stored in the `appsettings.json` file. This file can serve as a template for the `appsettings.Development.json` file, which can be used to override the settings for local development. The only settings that needs to be setup is `ConnectionStrings:EmployeesDb` which should contain the connection string to the database. If using the docker-compose setup, the connection string should be `Host=employee-management-db; Port=5432;Database=employee-management;Username=employee-management;Password=employee-management`