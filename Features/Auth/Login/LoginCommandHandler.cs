namespace DIMS_Backend.Features.Auth.Login;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models; // Asegúrate de que apunte a tus modelos generados
using DIMS_Backend.Infrastructure.Security;
using DIMS_Backend.Common;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResultDto>>
{
    private readonly UcbPortalContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginCommandHandler(
        UcbPortalContext context,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar al usuario por correo electrónico
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        // 2. Validar si existe, si está activo y si la contraseña coincide
        // Nota: Lanzamos excepciones generales por seguridad (no dar pistas a atacantes)
        if (user == null || user.Activo == false)
        {
            return Result<AuthResultDto>.Failure(new Error("Auth.InvalidCredentials", "Credenciales incorrectas o cuenta inactiva."));
        }

        bool isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return Result<AuthResultDto>.Failure(new Error("Auth.InvalidCredentials", "Credenciales incorrectas o cuenta inactiva."));
        }

        // 3. Generar Access Token (15 min) y Refresh Token (7 días)
        string accessToken = _jwtProvider.GenerateAccessToken(user);
        string refreshToken = _jwtProvider.GenerateRefreshToken(user);

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