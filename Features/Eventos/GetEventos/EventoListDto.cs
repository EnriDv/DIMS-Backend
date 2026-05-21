namespace DIMS_Backend.Features.Eventos.GetEventos;

public class EventoListDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? DescripcionCorta { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime FechaEvento { get; set; }
    public string? Ubicacion { get; set; }
    public string? Lugar { get; set; }
    public string? ImagenUrl { get; set; }
    public string Tipo { get; set; } = string.Empty; // Taller, Conferencia, etc.
    public int? CarreraId { get; set; }
    public string? CarreraNombre { get; set; }
    public int Capacidad { get; set; }
    public int Inscritos { get; set; }
    public bool Publicado { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}