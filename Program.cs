using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using DIMS_Backend.Models;
using DIMS_Backend.Infrastructure.Security;
using Serilog;
using Serilog.Formatting.Compact;
using Amazon.S3;
using DIMS_Backend.Infrastructure.BackgroundServices;

// Cargar variables de entorno desde archivo .env SOLO en desarrollo local (no en Docker) si existe
if (!File.Exists("/.dockerenv") && File.Exists(".env"))
{
    Env.Load();
}

// Configurar Logger inicial para capturar problemas de arranque
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando el servidor web de DIMS-Backend...");

    var builder = WebApplication.CreateBuilder(args);

    // Configurar Serilog para logs estructurados
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();

        if (context.HostingEnvironment.IsDevelopment())
        {
            configuration.WriteTo.Console();
        }
        else
        {
            configuration.WriteTo.Console(new CompactJsonFormatter());
        }
    });

    // Construir connection string desde variables de entorno
    var dbHost = builder.Configuration["DB_HOST"] ?? "localhost";
    var dbPort = builder.Configuration["DB_PORT"] ?? "5432";
    var dbName = builder.Configuration["DB_NAME"] ?? "DIMS";
    var dbUsername = builder.Configuration["DB_USERNAME"] ?? "postgres";
    var dbPassword = builder.Configuration["DB_PASSWORD"] ?? "";

    var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUsername};Password={dbPassword}";

    // Conditionally configure database provider to prevent provider conflicts in tests
    builder.Services.AddDbContext<UcbPortalContext>(options =>
    {
        if (builder.Configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            options.UseInMemoryDatabase("InMemoryDbForTesting");
        }
        else
        {
            options.UseNpgsql(connectionString);
        }
    });

    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
    builder.Services.AddScoped<IJwtProvider, JwtProvider>();

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

    // Registrar servicios de AWS y Background S3 Worker
    builder.Services.AddAWSService<IAmazonS3>();
    builder.Services.AddSingleton<S3BackgroundQueue>();
    builder.Services.AddHostedService<S3BackgroundService>();

    // Registrar Manejador Global de Excepciones y Health Checks
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddHealthChecks();

    // ==========================================
    // 4. AUTENTICACIÓN Y JWT
    // ==========================================
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            var jwtSecretKey = builder.Configuration["JWT_SECRET_KEY"] ?? builder.Configuration["Jwt:SecretKey"];
            var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? builder.Configuration["Jwt:Issuer"];
            var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? builder.Configuration["Jwt:Audience"];

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSecretKey!))
            };
        });

    builder.Services.AddAuthorization();

    // ==========================================
    // 5. CORS (Permitir conexión al Frontend de React/Angular/Astro)
    // ==========================================
    builder.Services.AddCors(options =>
    {
        var corsOriginsRaw = builder.Configuration["CORS_ORIGINS"];
        if (string.IsNullOrWhiteSpace(corsOriginsRaw))
        {
            corsOriginsRaw = "http://localhost:3000";
        }

        var corsOrigins = corsOriginsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        options.AddPolicy("AllowFrontend",
            policy => policy
                .WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod());
    });

    builder.Services.AddControllers();

    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Usar ExceptionHandler al inicio del pipeline
    app.UseExceptionHandler();

    // Registrar Request Logging de Serilog para capturar automáticamente requests completados con metadata (status, latency, etc.)
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

        // Levanta la interfaz gráfica de Scalar en /scalar/v1
        app.MapScalarApiReference();
    }

    // CORS debe ir antes de Auth
    app.UseCors("AllowFrontend");

    // Autenticación antes que Autorización
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Mapear endpoints de Health Checks
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"STARTUP CRASH: {ex}");
    Log.Fatal(ex, "El servidor web de DIMS-Backend terminó inesperadamente.");
    throw;
}
finally
{
    Log.Information("Apagando el servidor web de DIMS-Backend...");
    Log.CloseAndFlush();
}
