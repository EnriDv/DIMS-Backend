namespace DIMS_Backend.Features.Personas.GetPersonas;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public class GetPersonasHandler : IRequestHandler<GetPersonasQuery, List<PersonaListDto>>
{
    private readonly UcbPortalContext _context;

    public GetPersonasHandler(UcbPortalContext context)
    {
        _context = context;
    }

    public async Task<List<PersonaListDto>> Handle(GetPersonasQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Personas
            .Include(p => p.Carrera) // Para obtener el nombre de su carrera base
            .Where(p => p.Activo == true)
            .AsQueryable();

        // Aplicamos el filtro si el frontend mandó un ID de carrera
        if (request.CarreraId.HasValue)
        {
            query = query.Where(p => p.CarreraId == request.CarreraId.Value);
        }

        return await query
            .OrderBy(p => p.Nombre)
            .Select(p => new PersonaListDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Rol = p.Rol,
                FotoUrl = p.FotoUrl,
                Especialidad = p.Especialidad,
                CarreraNombre = p.Carrera.Nombre
            })
            .ToListAsync(cancellationToken);
    }
}