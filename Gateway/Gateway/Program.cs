using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Voeg configuratie toe om ocelot.json te laden
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

Console.WriteLine("Loaded ocelot.json successfully!");

// Voeg JWT authenticatie toe
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false; // Alleen voor dev-omgeving
        options.Authority = "http://keycloak:8080/realms/UserService"; // Keycloak Realm URL
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "http://keycloak:8080/realms/UserService",
            ValidateAudience = true,
            ValidAudience = "realm-management", // Audience in de JWT
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                using var httpClient = new HttpClient();
                var response = httpClient.GetStringAsync("http://keycloak:8080/realms/UserService/protocol/openid-connect/certs").Result;
                var keys = new JsonWebKeySet(response).GetSigningKeys();
                return keys;
            }
        };
    });

builder.Services.AddOcelot();

var app = builder.Build();


//app.UseAuthentication();
//app.UseAuthorization();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Downstream URL: {context.Request.Path}");
    await next.Invoke();
});

// Middleware voor authenticatie en autorisatie
// Custom middleware voor role-based validatie
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/user/login") ||
        context.Request.Path.StartsWithSegments("/user/register")) 
    {
        await next.Invoke();
        return;
    }

    if (!context.User.Identity.IsAuthenticated)
    {
        context.Response.StatusCode = 401; // Unauthorized
        await context.Response.WriteAsync("Authentication required.");
        return;
    }

    var realmAccessClaim = context.User.Claims.FirstOrDefault(c => c.Type == "realm_access");

    if (realmAccessClaim == null)
    {
        context.Response.StatusCode = 403; // Forbidden
        await context.Response.WriteAsync("Authorization failed. Missing 'realm_access' claim.");
        return;
    }

    try
    {
        var realmAccess = JsonDocument.Parse(realmAccessClaim.Value).RootElement;

        if (realmAccess.TryGetProperty("roles", out var roles))
        {
            foreach (var role in roles.EnumerateArray())
            {
                if (role.GetString() == "user")
                {
                    await next.Invoke();
                    return;
                }
            }
        }

        context.Response.StatusCode = 403; // Forbidden
        await context.Response.WriteAsync("Authorization failed. Missing required role: 'user'.");
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500; // Internal Server Error
        await context.Response.WriteAsync($"Error processing roles: {ex.Message}");
    }
});




await app.UseOcelot();
app.Run();

// UserRoleExtractor Class
public static class UserRoleExtractor
{
    public static bool HasUserRole(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            return false;
        }

        var token = authHeader.ToString().Replace("Bearer ", "").Trim();

        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                return false;
            }

            var jwtToken = handler.ReadJwtToken(token);

            var realmAccessClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
            if (string.IsNullOrEmpty(realmAccessClaim))
            {
                return false;
            }

            var realmAccess = JsonDocument.Parse(realmAccessClaim).RootElement;
            if (realmAccess.TryGetProperty("roles", out var roles))
            {
                foreach (var role in roles.EnumerateArray())
                {
                    if (role.GetString() == "user")
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing roles: {ex.Message}");
        }

        return false;
    }

}

