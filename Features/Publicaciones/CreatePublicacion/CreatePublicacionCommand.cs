namespace DIMS_Backend.Features.Publicaciones.CreatePublicacion;

using MediatR;

public class CreatePublicacionCommand : IRequest<int>
{
    public string Titulo { get; set; } = string.Empty;
    public string? Autor { get; set; }
    public string? Resumen { get; set; }
    public string? ArchivoUrl { get; set; }
    public int? CarreraId { get; set; }
    public string Tipo { get; set; } = "proyecto"; // tesis, articulo, libro
}