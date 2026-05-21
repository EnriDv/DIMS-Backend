// DIMS_Backend/Features/Eventos/CreateEvento/CreateEventoHandler.cs
using MediatR;
using DIMS_Backend.Models; // Tu clase Evento

public class CreateEventoHandler : IRequestHandler<CreateEventoCommand, int>
{
    private static readonly HashSet<string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "conferencia",
        "workshop",
        "feria",
        "charla",
        "seminario",
    };

    private readonly UcbPortalContext _context;

    public CreateEventoHandler(UcbPortalContext context) => _context = context;

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

    public async Task<int> Handle(CreateEventoCommand request, CancellationToken cancellationToken)
    {
        var nuevoEvento = new Evento
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            FechaEvento = ToTimestampWithoutTimeZone(request.FechaEvento),
            Lugar = request.Lugar,
            Tipo = NormalizarTipo(request.Tipo),
            CarreraId = request.CarreraId,
            Capacidad = request.Capacidad > 0 ? request.Capacidad : null,
            ImagenUrl = request.ImagenUrl,
            Inscritos = 0, // Empezamos en cero
            Publicado = true,
            CreatedBy = request.CreatedBy,
        };

        _context.Eventos.Add(nuevoEvento);
        await _context.SaveChangesAsync(cancellationToken);

        return nuevoEvento.Id;
    }
}