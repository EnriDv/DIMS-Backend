namespace DIMS_Backend.Features.Personas;

using MediatR;

public class CreatePersonaCommand : IRequest<int>
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = "docente"; // docente, director, etc.
    public int CarreraId { get; set; }
    public string? Especialidad { get; set; }
    public string? GradoAcademico { get; set; }
}