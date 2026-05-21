namespace DIMS_Backend.Features.Publicaciones.GetPublicaciones;

public class PublicacionDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Autor { get; set; }
    public string? Resumen { get; set; }
    public string? ArchivoUrl { get; set; }
    public DateTime? Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty; // proyecto, tesis, articulo
    public string CarreraNombre { get; set; } = string.Empty;
}