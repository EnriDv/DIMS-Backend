namespace DIMS_Backend.Features.Carreras.GetCarreras;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public class GetCarrerasHandler : IRequestHandler<GetCarrerasQuery, List<CarreraListDto>>
{
    private readonly UcbPortalContext _context;

    public GetCarrerasHandler(UcbPortalContext context)
    {
        _context = context;
    }

    public async Task<List<CarreraListDto>> Handle(GetCarrerasQuery request, CancellationToken cancellationToken)
    {
        // Consultamos la tabla Carreras, filtramos las activas y mapeamos al DTO
        var carreras = await _context.Carreras
            .Where(c => c.Activa == true)
            .OrderBy(c => c.Nombre) // Las ordenamos alfabéticamente
            .Select(c => new CarreraListDto
            {
                Id = c.Id,
                Slug = c.Slug,
                Nombre = c.Nombre,
                Duracion = c.Duracion,
                Modalidad = c.Modalidad,
                ImagenUrl = c.ImagenUrl,
                Color = c.Color,
                Icono = c.Icono
            })
            .ToListAsync(cancellationToken);

        return carreras;
    }
}