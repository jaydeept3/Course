using UserManagementAPI.Contracts;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var usersApi = app.MapGroup("/api/users");

usersApi.MapGet("/", (IUserRepository repository) =>
{
    return Results.Ok(repository.GetAll());
})
.WithName("GetUsers");

usersApi.MapGet("/{id:guid}", (Guid id, IUserRepository repository) =>
{
    var user = repository.GetById(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
})
.WithName("GetUserById");

usersApi.MapPost("/", (CreateUserRequest request, IUserRepository repository) =>
{
    var errors = ValidateCreateRequest(request);
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var user = new User
    {
        Id = Guid.NewGuid(),
        FirstName = request.FirstName.Trim(),
        LastName = request.LastName.Trim(),
        Email = request.Email.Trim(),
        Department = request.Department.Trim(),
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow
    };

    repository.Create(user);
    return Results.Created($"/api/users/{user.Id}", user);
})
.WithName("CreateUser");

usersApi.MapPut("/{id:guid}", (Guid id, UpdateUserRequest request, IUserRepository repository) =>
{
    var existing = repository.GetById(id);
    if (existing is null)
    {
        return Results.NotFound();
    }

    var errors = ValidateUpdateRequest(request);
    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    existing.FirstName = request.FirstName.Trim();
    existing.LastName = request.LastName.Trim();
    existing.Email = request.Email.Trim();
    existing.Department = request.Department.Trim();
    existing.IsActive = request.IsActive;

    repository.Update(existing);
    return Results.Ok(existing);
})
.WithName("UpdateUser");

usersApi.MapDelete("/{id:guid}", (Guid id, IUserRepository repository) =>
{
    var deleted = repository.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteUser");

app.Run();

static Dictionary<string, string[]> ValidateCreateRequest(CreateUserRequest request)
{
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(request.FirstName))
    {
        errors["firstName"] = ["First name is required."];
    }

    if (string.IsNullOrWhiteSpace(request.LastName))
    {
        errors["lastName"] = ["Last name is required."];
    }

    if (string.IsNullOrWhiteSpace(request.Department))
    {
        errors["department"] = ["Department is required."];
    }

    if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
    {
        errors["email"] = ["A valid email is required."];
    }

    return errors;
}

static Dictionary<string, string[]> ValidateUpdateRequest(UpdateUserRequest request)
{
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(request.FirstName))
    {
        errors["firstName"] = ["First name is required."];
    }

    if (string.IsNullOrWhiteSpace(request.LastName))
    {
        errors["lastName"] = ["Last name is required."];
    }

    if (string.IsNullOrWhiteSpace(request.Department))
    {
        errors["department"] = ["Department is required."];
    }

    if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
    {
        errors["email"] = ["A valid email is required."];
    }

    return errors;
}
