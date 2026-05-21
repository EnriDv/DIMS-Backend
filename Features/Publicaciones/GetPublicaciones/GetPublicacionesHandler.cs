namespace DIMS_Backend.Features.Publicaciones.GetPublicaciones;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public record GetPublicacionesQuery(int? CarreraId, string? Tipo) : IRequest<List<PublicacionDto>>;

public class GetPublicacionesHandler : IRequestHandler<GetPublicacionesQuery, List<PublicacionDto>>
{
    private readonly UcbPortalContext _context;
    public GetPublicacionesHandler(UcbPortalContext context) => _context = context;

    public async Task<List<PublicacionDto>> Handle(GetPublicacionesQuery request, CancellationToken ct)
    {
        var query = _context.Publicaciones
            .Include(p => p.Carrera)
            .AsQueryable();

        if (request.CarreraId.HasValue)
            query = query.Where(p => p.CarreraId == request.CarreraId);

        if (!string.IsNullOrEmpty(request.Tipo))
            query = query.Where(p => p.Tipo == request.Tipo);

        return await query
            .OrderByDescending(p => p.FechaPublicacion)
            .Select(p => new PublicacionDto
            {
                Id = p.Id,
                Titulo = p.Titulo,
                Autor = p.Autor,
                Resumen = p.Resumen,
                ArchivoUrl = p.ArchivoUrl,
                Fecha = p.FechaPublicacion != null ? p.FechaPublicacion.Value.ToDateTime(TimeOnly.MinValue) : null,
                Tipo = p.Tipo ?? "Otro",
                CarreraNombre = p.Carrera != null ? p.Carrera.Nombre : "General"
            }).ToListAsync(ct);
    }
}