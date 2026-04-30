namespace DIMS_Backend.Features.Auth.Logout;

using MediatR;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutResultDto>
{
    public Task<LogoutResultDto> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // En una arquitectura stateless, el logout es simplemente confirmación.
        // El frontend elimina los tokens del localStorage.
        // El servidor no necesita hacer nada especial.
        
        return Task.FromResult(new LogoutResultDto
        {
            Message = "Sesión cerrada correctamente. Los tokens han sido invalidados."
        });
    }
}
