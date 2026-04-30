namespace DIMS_Backend.Features.Noticias.GetNoticias;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public record GetNoticiasQuery(int? CarreraId, bool IncludeUnpublished = false) : IRequest<List<NoticiaDto>>;

public class GetNoticiasHandler : IRequestHandler<GetNoticiasQuery, List<NoticiaDto>>
{
    private readonly UcbPortalContext _context;
    public GetNoticiasHandler(UcbPortalContext context) => _context = context;

    public async Task<List<NoticiaDto>> Handle(GetNoticiasQuery request, CancellationToken ct)
    {
        var query = _context.Noticias
            .Include(n => n.Carrera)
            .AsQueryable();

        if (!request.IncludeUnpublished)
        {
            query = query.Where(n => n.Publicada == true);
        }

        if (request.CarreraId.HasValue)
            query = query.Where(n => n.CarreraId == request.CarreraId);

        return await query
            .OrderByDescending(n => n.Destacada)
            .ThenByDescending(n => n.Fecha)
            .Select(n => new NoticiaDto
            {
                Id = n.Id,
                Titulo = n.Titulo,
                Contenido = n.Contenido,
                Fecha = n.Fecha,
                ImagenUrl = n.ImagenUrl,
                CarreraId = n.CarreraId,
                CarreraNombre = n.Carrera != null ? n.Carrera.Nombre : "General",
                Destacada = n.Destacada ?? false,
                Publicada = n.Publicada ?? false,
                CreatedBy = n.CreatedBy,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
            }).ToListAsync(ct);
    }
}