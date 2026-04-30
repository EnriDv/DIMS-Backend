namespace DIMS_Backend.Features.Noticias.GetNoticias;

public class NoticiaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty; // Puede ser HTML
    public DateTime Fecha { get; set; }
    public string? ImagenUrl { get; set; }
    public int? CarreraId { get; set; }
    public string? CarreraNombre { get; set; } // "General" si CarreraId es null
    public bool Destacada { get; set; }
    public bool Publicada { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}