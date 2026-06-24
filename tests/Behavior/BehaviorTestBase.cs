using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using DIMS_Backend.Models;
using DIMS_Backend.Infrastructure.Security;
using System.Net.Http;
using System.Linq;
using System;
using Amazon.S3;
using Amazon.S3.Model;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using DIMS_Backend.Infrastructure.Messaging;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace DIMS_Backend.Tests.Behavior;

public class FakeS3Proxy : DispatchProxy
{
    public bool ShouldFail { get; set; } = false;

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod?.Name == "Dispose")
        {
            return null;
        }

        if (targetMethod?.Name == "PutObjectAsync")
        {
            if (ShouldFail)
            {
                throw new AmazonS3Exception("Simulated S3 failure");
            }
            return Task.FromResult(new PutObjectResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            });
        }
        
        throw new NotImplementedException($"Method {targetMethod?.Name} is not implemented in FakeS3Proxy.");
    }

    public static IAmazonS3 Create(bool shouldFail = false)
    {
        object proxy = Create<IAmazonS3, FakeS3Proxy>();
        ((FakeS3Proxy)proxy).ShouldFail = shouldFail;
        return (IAmazonS3)proxy;
    }
}

file sealed class NoopSqsService : ISqsService
{
    public Task SendMessageAsync(string messageBody, System.Threading.CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

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
            // Replace IAmazonS3 service to avoid credential resolution crashes
            var s3Descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IAmazonS3));
            if (s3Descriptor != null)
            {
                services.Remove(s3Descriptor);
            }
            services.AddSingleton<IAmazonS3>(sp => FakeS3Proxy.Create(shouldFail: false));

            // Reemplazar ISqsService con un stub que no hace nada en tests
            var sqsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISqsService));
            if (sqsDescriptor != null) services.Remove(sqsDescriptor);
            services.AddScoped<ISqsService, NoopSqsService>();
        });

        // Set configuration variables to avoid crashes in Program.cs
        builder.UseSetting("UseInMemoryDatabase", "true");
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
