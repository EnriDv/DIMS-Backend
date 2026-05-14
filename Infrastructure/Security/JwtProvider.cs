namespace DIMS_Backend.Infrastructure.Security;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DIMS_Backend.Models; // Asegúrate de que este namespace sea el correcto

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
}

public sealed class JwtProvider : IJwtProvider
{
    private readonly IConfiguration _configuration;

    public JwtProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Genera un Access Token válido por 15 minutos (para acceder a recursos)
    /// </summary>
    public string GenerateAccessToken(User user)
    {
        return GenerateToken(user, TimeSpan.FromMinutes(15));
    }

    /// <summary>
    /// Genera un Refresh Token válido por 7 días (solo para renovar access tokens)
    /// </summary>
    public string GenerateRefreshToken(User user)
    {
        return GenerateToken(user, TimeSpan.FromDays(7));
    }

    private string GenerateToken(User user, TimeSpan expiresIn)
    {
        // 1. Obtener las llaves de configuración
        var secretKey = _configuration["JWT_SECRET_KEY"] ?? _configuration["Jwt:SecretKey"]
            ?? throw new ArgumentNullException("JWT_SECRET_KEY no está configurado");

        var issuer = _configuration["JWT_ISSUER"] ?? _configuration["Jwt:Issuer"]
            ?? throw new ArgumentNullException("JWT_ISSUER no está configurado");

        var audience = _configuration["JWT_AUDIENCE"] ?? _configuration["Jwt:Audience"]
            ?? throw new ArgumentNullException("JWT_AUDIENCE no está configurado");

        // 2. Configurar la firma digital
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 3. Crear los "Claims"
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("nombre", user.Nombre),
            new Claim(ClaimTypes.Role, user.Rol)
        };

        // 4. Construir el Token con la duración especificada
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(expiresIn),
            signingCredentials: credentials);

        // 5. Retornar el token como un string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}