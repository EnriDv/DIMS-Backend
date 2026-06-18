using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using DIMS_Backend.Models;
using DIMS_Backend.Infrastructure.Security;
using System.Net.Http;
using System.Linq;
using System;

namespace DIMS_Backend.Tests.Behavior;

public class BehaviorTestBase : WebApplicationFactory<Program>
{
    public BehaviorTestBase()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Remove the descriptor for UcbPortalContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<UcbPortalContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register an InMemory database instead
            services.AddDbContext<UcbPortalContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
        });

        // Set configuration variables to avoid crashes in Program.cs
        builder.UseSetting("CORS_ORIGINS", "http://localhost:3000");
        builder.UseSetting("JWT_SECRET_KEY", "super_secret_key_1234567890_super_secret_key_testing");
        builder.UseSetting("JWT_ISSUER", "TestIssuer");
        builder.UseSetting("JWT_AUDIENCE", "TestAudience");
    }

    public HttpClient CreateTestClient()
    {
        return CreateClient();
    }

    public string GenerateTestToken(Guid userId, string email, string role)
    {
        using var scope = Services.CreateScope();
        var jwtProvider = scope.ServiceProvider.GetRequiredService<IJwtProvider>();
        
        var user = new User
        {
            Id = userId,
            Email = email,
            Nombre = "Test Behavior User",
            Rol = role,
            Activo = true
        };
        
        return jwtProvider.GenerateAccessToken(user);
    }
}
