namespace DIMS_Backend.Features.Auth.Register;

using MediatR;
using DIMS_Backend.Common;

public record RegisterCommand(
    string Nombre,
    string Email,
    string Password,
    string Rol = "estudiante"
) : IRequest<Result<Guid>>;