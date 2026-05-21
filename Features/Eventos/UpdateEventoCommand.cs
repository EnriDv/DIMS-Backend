namespace DIMS_Backend.Features.Eventos;

using MediatR;

public class UpdateEventoCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaEvento { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? CarreraId { get; set; }
    public string? ImagenUrl { get; set; }
    public int Capacidad { get; set; }
    public bool Publicado { get; set; } = true;
}

public record DeleteEventoCommand(int Id) : IRequest<bool>;