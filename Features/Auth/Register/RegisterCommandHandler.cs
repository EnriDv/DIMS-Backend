namespace DIMS_Backend.Features.Auth.Register;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Infrastructure.Security;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly UcbPortalContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(UcbPortalContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Verificar si el correo ya existe para no duplicar
        var existeUser = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (existeUser)
        {
            throw new ArgumentException("El correo ya está registrado.");
        }

        var user = new User
        {
            // El Id se genera automáticamente por el default uuid_generate_v4() en tu BD, 
            // pero si tu modelo de EF Core no lo tiene configurado así, puedes usar Guid.NewGuid()
            Nombre = request.Nombre,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Rol = request.Rol,
            Activo = true
            // created_at y updated_at se manejan en BD gracias a tus Triggers y Defaults
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}