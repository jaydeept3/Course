# UserManagementAPI

User management API for TechHive Solutions internal HR and IT tooling.

## What was built

- ASP.NET Core Web API project named `UserManagementAPI`
- In-memory user store using a repository pattern
- Middleware for request/response audit logging
- Middleware for standardized global exception handling
- Token-based authentication protecting all user endpoints
- Full CRUD endpoints for users:
  - `GET /api/users`
  - `GET /api/users/{id}`
  - `POST /api/users`
  - `PUT /api/users/{id}`
  - `DELETE /api/users/{id}`
- Request validation for create and update operations
- `.http` request file for quick endpoint testing

## Project structure

- `Program.cs`: API setup and endpoint mappings
- `Models/User.cs`: user entity model
- `Contracts/CreateUserRequest.cs`: create payload contract
- `Contracts/UpdateUserRequest.cs`: update payload contract
- `Repositories/IUserRepository.cs`: repository contract
- `Repositories/InMemoryUserRepository.cs`: in-memory repository implementation
- `Middleware/RequestResponseLoggingMiddleware.cs`: audit logging middleware
- `Middleware/GlobalExceptionMiddleware.cs`: centralized exception handling
- `Security/StaticTokenAuthenticationHandler.cs`: token authentication handler
- `UserManagementAPI.http`: CRUD endpoint test requests

## Run locally

```bash
dotnet restore
dotnet run --urls http://localhost:5055
```

## Test CRUD endpoints (Postman or VS Code REST Client)

Use Bearer token authentication for all requests.

- Header: `Authorization: Bearer techhive-dev-token`
- Config location: `appsettings.json` and `appsettings.Development.json` under `Authentication:Token`

1. `POST /api/users` to create a user.
2. Copy the returned `id`.
3. `GET /api/users/{id}` to verify retrieval.
4. `PUT /api/users/{id}` to update user details.
5. `GET /api/users` to verify list retrieval.
6. `DELETE /api/users/{id}` to remove the user.
7. `GET /api/users/{id}` should return `404 Not Found` after deletion.

You can also run the prebuilt requests in `UserManagementAPI.http`.

## How Microsoft Copilot helped

- Generated and refined minimal API endpoint boilerplate for CRUD patterns.
- Suggested clean separation of concerns with model, contracts, and repository classes.
- Accelerated validation logic generation for user input fields.
- Helped produce ready-to-run HTTP request examples for endpoint testing.
- Assisted in tightening endpoint behaviors (`201 Created`, `404 Not Found`, `204 No Content`, validation failures).

## Notes

- Data is stored in-memory and resets when the app restarts.
- This is suitable for development and internal prototyping.
- Replace the development token with a secure secret source (environment variables, key vault, or secret manager) before production use.
