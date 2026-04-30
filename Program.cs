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

builder.Services.AddDbContext<UcbPortalContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// ==========================================
// 4. AUTENTICACIÓN Y JWT
// ==========================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization();

// ==========================================
// 5. CORS (Permitir conexión al Frontend de React/Angular/Astro)
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:4321", "http://localhost:4200")
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

app.UseHttpsRedirection();

// CORS debe ir antes de Auth
app.UseCors("AllowFrontend");

// Autenticación antes que Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();