namespace DIMS_Backend.Features.Noticias.GetNoticiaById;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Features.Noticias.GetNoticias;

public record GetNoticiaByIdQuery(int Id) : IRequest<NoticiaDto?>;

public class GetNoticiaByIdHandler : IRequestHandler<GetNoticiaByIdQuery, NoticiaDto?>
{
    private readonly UcbPortalContext _context;
    public GetNoticiaByIdHandler(UcbPortalContext context) => _context = context;

    public async Task<NoticiaDto?> Handle(GetNoticiaByIdQuery request, CancellationToken ct)
    {
        return await _context.Noticias
            .Include(n => n.Carrera)
            .Where(n => n.Id == request.Id)
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
            })
            .FirstOrDefaultAsync(ct);
    }
}
