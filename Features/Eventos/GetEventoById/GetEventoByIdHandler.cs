namespace DIMS_Backend.Features.Eventos.GetEventoById;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Features.Eventos.GetEventos;

public record GetEventoByIdQuery(int Id) : IRequest<EventoListDto?>;

public class GetEventoByIdHandler : IRequestHandler<GetEventoByIdQuery, EventoListDto?>
{
    private readonly UcbPortalContext _context;
    public GetEventoByIdHandler(UcbPortalContext context) => _context = context;

    public async Task<EventoListDto?> Handle(GetEventoByIdQuery request, CancellationToken ct)
    {
        return await _context.Eventos
            .Include(e => e.Carrera)
            .Where(e => e.Id == request.Id)
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
            })
            .FirstOrDefaultAsync(ct);
    }
}
