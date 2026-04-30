namespace DIMS_Backend.Features.Auth.Logout;

using MediatR;

public class LogoutCommand : IRequest<LogoutResultDto>
{
}

public class LogoutResultDto
{
    public string Message { get; set; } = "Sesión cerrada correctamente.";
}
