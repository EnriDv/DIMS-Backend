using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using DIMS_Backend.Models;
using DIMS_Backend.Infrastructure.Security;

// Cargar variables de entorno desde archivo .env SOLO en desarrollo local (no en Docker)
if (!File.Exists("/.dockerenv"))
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

// Construir connection string desde variables de entorno
var dbHost = builder.Configuration["DB_HOST"] ?? "localhost";
var dbPort = builder.Configuration["DB_PORT"] ?? "5432";
var dbName = builder.Configuration["DB_NAME"] ?? "DIMS";
var dbUsername = builder.Configuration["DB_USERNAME"] ?? "postgres";
var dbPassword = builder.Configuration["DB_PASSWORD"] ?? "";

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUsername};Password={dbPassword}";

builder.Services.AddDbContext<UcbPortalContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

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
        throw new InvalidOperationException("CORS_ORIGINS is required and must be a comma-separated list of allowed origins.");
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

app.Run();