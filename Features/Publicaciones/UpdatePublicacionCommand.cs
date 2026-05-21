namespace DIMS_Backend.Features.Publicaciones;

using MediatR;

public class UpdatePublicacionCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Autor { get; set; }
    public string? Resumen { get; set; }
    public string? ArchivoUrl { get; set; }
    public int? CarreraId { get; set; }
    public string Tipo { get; set; } = "proyecto";

    // Datos de seguridad (vienen del Token)
    public Guid RequestUserId { get; set; }
    public string RequestUserRole { get; set; } = string.Empty;
}