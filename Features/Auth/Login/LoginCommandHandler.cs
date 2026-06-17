namespace DIMS_Backend.Features.Auth.Login;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Infrastructure.Security;
using DIMS_Backend.Common;
using Microsoft.Extensions.Logging;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResultDto>>
{
    private readonly UcbPortalContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UcbPortalContext context,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async Task<Result<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Intento de inicio de sesión para el correo: {Email}", request.Email);

        // 1. Buscar al usuario por correo electrónico
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // 2. Validar si existe, si está activo y si la contraseña coincide
        if (user == null || user.Activo == false)
        {
            _logger.LogWarning("Inicio de sesión fallido para el correo: {Email}. Razón: Usuario no encontrado o cuenta inactiva.", request.Email);
            return Result<AuthResultDto>.Failure(new Error("Auth.InvalidCredentials", "Credenciales incorrectas o cuenta inactiva."));
        }

        bool isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Inicio de sesión fallido para el correo: {Email}. Razón: Contraseña incorrecta.", request.Email);
            return Result<AuthResultDto>.Failure(new Error("Auth.InvalidCredentials", "Credenciales incorrectas o cuenta inactiva."));
        }

        // 3. Generar Access Token (15 min) y Refresh Token (7 días)
        string accessToken = _jwtProvider.GenerateAccessToken(user);
        string refreshToken = _jwtProvider.GenerateRefreshToken(user);

        _logger.LogInformation("Inicio de sesión exitoso para el usuario: {UserId} ({Email}) con rol: {Rol}", user.Id, user.Email, user.Rol);

        // 4. Retornar los datos al frontend
        return Result<AuthResultDto>.Success(new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            Nombre = user.Nombre,
            Email = user.Email,
            Rol = user.Rol
        });
    }
}
