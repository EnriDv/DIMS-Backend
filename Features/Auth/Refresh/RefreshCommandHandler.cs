namespace DIMS_Backend.Features.Auth.Refresh;

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DIMS_Backend.Features.Auth.Login;
using DIMS_Backend.Infrastructure.Security;
using DIMS_Backend.Models;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshTokenResultDto>
{
    private readonly UcbPortalContext _context;
    private readonly IJwtProvider _jwtProvider;
    private readonly IConfiguration _configuration;

    public RefreshCommandHandler(
        UcbPortalContext context,
        IJwtProvider jwtProvider,
        IConfiguration configuration)
    {
        _context = context;
        _jwtProvider = jwtProvider;
        _configuration = configuration;
    }

    public async Task<RefreshTokenResultDto> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar y extraer el userid del refresh token
        var userId = ValidateRefreshToken(request.RefreshToken);

        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Refresh token inválido o expirado.");
        }

        // 2. Buscar el usuario en la base de datos
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken: cancellationToken);

        if (user == null || user.Activo == false)
        {
            throw new UnauthorizedAccessException("Usuario no encontrado o inactivo.");
        }

        // 3. Generar nuevos tokens (token rotation)
        string newAccessToken = _jwtProvider.GenerateAccessToken(user);
        string newRefreshToken = _jwtProvider.GenerateRefreshToken(user);

        // 4. Retornar los nuevos tokens
        return new RefreshTokenResultDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    /// <summary>
    /// Valida un Refresh Token JWT y extrae el userId.
    /// Retorna Guid.Empty si el token es inválido.
    /// </summary>
    private Guid ValidateRefreshToken(string token)
    {
        try
        {
            var secretKey = _configuration["Jwt:SecretKey"]
                ?? throw new ArgumentNullException("Jwt:SecretKey no está configurado");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Validar con validación de expiración
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            // Extraer el "sub" claim que contiene el userId
            var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return Guid.Empty;
        }
        catch
        {
            // Token inválido, expirado o malformado
            return Guid.Empty;
        }
    }
}
