namespace DIMS_Backend.Features.Auth.Register;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Infrastructure.Security;
using DIMS_Backend.Common;
using Microsoft.Extensions.Logging;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<Guid>>
{
    private readonly UcbPortalContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        UcbPortalContext context, 
        IPasswordHasher passwordHasher,
        ILogger<RegisterCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Intento de registro de nuevo usuario con correo: {Email}", request.Email);

        // Verificar si el correo ya existe para no duplicar
        var existeUser = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (existeUser)
        {
            _logger.LogWarning("Registro fallido. El correo ya está registrado: {Email}", request.Email);
            return Result<Guid>.Failure(new Error("Auth.DuplicateEmail", "El correo ya está registrado."));
        }

        var user = new User
        {
            Nombre = request.Nombre,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Rol = request.Rol,
            Activo = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Usuario registrado con éxito. UserId: {UserId}, Email: {Email}, Rol: {Rol}", user.Id, user.Email, user.Rol);

        return Result<Guid>.Success(user.Id);
    }
}
