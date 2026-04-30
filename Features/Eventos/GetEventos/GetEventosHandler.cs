namespace DIMS_Backend.Features.Eventos.GetEventos;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public record GetEventosQuery(int? CarreraId = null, bool IncludeUnpublished = false, bool SoloProximos = true) : IRequest<List<EventoListDto>>;

public class GetEventosHandler : IRequestHandler<GetEventosQuery, List<EventoListDto>>
{
    private readonly UcbPortalContext _context;

    public GetEventosHandler(UcbPortalContext context) => _context = context;

    public async Task<List<EventoListDto>> Handle(GetEventosQuery request, CancellationToken ct)
    {
        var fechaActual = DateTime.Now;

        var query = _context.Eventos
            .Include(e => e.Carrera)
            .AsQueryable();

        if (!request.IncludeUnpublished)
        {
            query = query.Where(e => e.Publicado == true);
        }

        if (request.SoloProximos)
        {
            query = query.Where(e => e.FechaEvento >= fechaActual);
        }

        if (request.CarreraId.HasValue)
        {
            query = query.Where(e => e.CarreraId == request.CarreraId);
        }

        return await query
            .OrderBy(e => e.FechaEvento)
            .Select(e => new EventoListDto
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DescripcionCorta = e.Descripcion != null && e.Descripcion.Length > 100
                                   ? e.Descripcion.Substring(0, 100) + "..."
                                   : e.Descripcion,
                Fecha = e.FechaEvento,
                FechaEvento = e.FechaEvento,
                Ubicacion = e.Lugar,
                Lugar = e.Lugar,
                ImagenUrl = e.ImagenUrl,
                Tipo = e.Tipo,
                CarreraId = e.CarreraId,
                CarreraNombre = e.Carrera != null ? e.Carrera.Nombre : "General",
                Capacidad = e.Capacidad ?? 0,
                Inscritos = e.Inscritos ?? 0,
                Publicado = e.Publicado ?? false,
                CreatedBy = e.CreatedBy,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
            }).ToListAsync(ct);
    }
}