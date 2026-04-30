namespace DIMS_Backend.Features.Eventos;

using DIMS_Backend.Models;
using MediatR;

public class UpdateEventoHandler(UcbPortalContext context) : IRequestHandler<UpdateEventoCommand, bool>
{
    private static readonly HashSet<string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "conferencia",
        "workshop",
        "feria",
        "charla",
        "seminario",
    };

    private static DateTime ToTimestampWithoutTimeZone(DateTime value)
        => DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

    private static string NormalizarTipo(string? tipo)
    {
        var normalizado = (tipo ?? string.Empty).Trim().ToLowerInvariant();
        if (!TiposPermitidos.Contains(normalizado))
        {
            throw new ArgumentException("Tipo de evento no valido.");
        }

        return normalizado;
    }

    public async Task<bool> Handle(UpdateEventoCommand request, CancellationToken cancellationToken)
    {
        var evento = await context.Eventos.FindAsync(request.Id);
        if (evento == null)
        {
            return false;
        }

        evento.Titulo = request.Titulo;
        evento.Descripcion = request.Descripcion;
        evento.FechaEvento = ToTimestampWithoutTimeZone(request.FechaEvento);
        evento.Lugar = request.Lugar;
        evento.Tipo = NormalizarTipo(request.Tipo);
        evento.CarreraId = request.CarreraId;
        evento.ImagenUrl = request.ImagenUrl;
        evento.Capacidad = request.Capacidad > 0 ? request.Capacidad : null;
        evento.Publicado = request.Publicado;
        evento.UpdatedAt = ToTimestampWithoutTimeZone(DateTime.UtcNow);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
