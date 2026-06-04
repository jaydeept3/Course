using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace UserManagementAPI.Security;

public class StaticTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    public StaticTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header."));
        }

        var authHeader = authHeaderValue.ToString();
        const string bearerPrefix = "Bearer ";

        if (!authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization header is not a Bearer token."));
        }

        var providedToken = authHeader[bearerPrefix.Length..].Trim();
        var expectedToken = _configuration["Authentication:Token"];

        if (string.IsNullOrWhiteSpace(expectedToken))
        {
            return Task.FromResult(AuthenticateResult.Fail("Server authentication token is not configured."));
        }

        if (!string.Equals(providedToken, expectedToken, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid token."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "internal-client"),
            new Claim(ClaimTypes.Name, "TechHiveClient"),
            new Claim(ClaimTypes.Role, "ApiClient")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
