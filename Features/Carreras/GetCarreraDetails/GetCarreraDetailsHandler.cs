namespace DIMS_Backend.Features.Carreras.GetCarreraDetails;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public class GetCarreraDetailsHandler : IRequestHandler<GetCarreraDetailsQuery, CarreraDetailDto>
{
    private readonly UcbPortalContext _context;

    public GetCarreraDetailsHandler(UcbPortalContext context)
    {
        _context = context;
    }

    public async Task<CarreraDetailDto> Handle(GetCarreraDetailsQuery request, CancellationToken cancellationToken)
    {
        var carrera = await _context.Carreras
            .Include(c => c.PerfilEgresados) // <-- JOIN con la tabla de perfil
            .Include(c => c.CampoLaborals)   // <-- JOIN con la tabla de campo laboral
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.Activa == true, cancellationToken);

        if (carrera == null)
        {
            throw new KeyNotFoundException($"No se encontró la carrera con ID {request.Id} o está inactiva.");
        }

        return new CarreraDetailDto
        {
            Id = carrera.Id,
            Slug = carrera.Slug,
            Nombre = carrera.Nombre,
            Descripcion = carrera.Descripcion ?? "",
            Duracion = carrera.Duracion,
            Modalidad = carrera.Modalidad,
            ImagenUrl = carrera.ImagenUrl,
            Color = carrera.Color,
            Icono = carrera.Icono,
            // Ordenamos y extraemos solo la descripción para tener un array limpio de strings
            PerfilEgresado = carrera.PerfilEgresados.OrderBy(p => p.Orden).Select(p => p.Descripcion).ToList(),
            CampoLaboral = carrera.CampoLaborals.OrderBy(c => c.Orden).Select(c => c.Descripcion).ToList()
        };
    }
}