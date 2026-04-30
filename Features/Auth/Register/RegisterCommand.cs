namespace DIMS_Backend.Features.Auth.Register;

using MediatR;

public class RegisterCommand : IRequest<Guid>
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Rol { get; set; } = "admin"; // Por defecto lo haremos admin
}