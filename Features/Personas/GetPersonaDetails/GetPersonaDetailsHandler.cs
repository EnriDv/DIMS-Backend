namespace DIMS_Backend.Features.Personas.GetPersonaDetails;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public class GetPersonaDetailsHandler : IRequestHandler<GetPersonaDetailsQuery, PersonaDetailDto>
{
    private readonly UcbPortalContext _context;

    public GetPersonaDetailsHandler(UcbPortalContext context)
    {
        _context = context;
    }

    public async Task<PersonaDetailDto> Handle(GetPersonaDetailsQuery request, CancellationToken cancellationToken)
    {
        var persona = await _context.Personas
            .Include(p => p.Carrera)
            .Include(p => p.Materia) // Relación N:N con materias
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.Activo == true, cancellationToken);

        if (persona == null)
        {
            throw new KeyNotFoundException($"No se encontró al docente/director con ID {request.Id}.");
        }

        // Extraemos las áreas únicas de las materias que dicta (ignorando nulos)
        var areasUnicas = persona.Materia
            .Where(m => !string.IsNullOrEmpty(m.Area))
            .Select(m => m.Area!)
            .Distinct()
            .ToList();

        return new PersonaDetailDto
        {
            Id = persona.Id,
            Nombre = persona.Nombre,
            Email = persona.Email,
            Rol = persona.Rol,
            Bio = persona.Bio,
            FotoUrl = persona.FotoUrl,
            Especialidad = persona.Especialidad,
            GradoAcademico = persona.GradoAcademico,
            LinkedinUrl = persona.LinkedinUrl,
            GoogleScholarUrl = persona.GoogleScholarUrl,
            CarreraBase = persona.Carrera.Nombre,
            Areas = areasUnicas, // <-- Aquí insertamos las etiquetas calculadas
            Materias = persona.Materia.Select(m => new MateriaDocenteDto
            {
                Id = m.Id,
                Sigla = m.Sigla,
                Nombre = m.Nombre,
                Area = m.Area
            }).ToList()
        };
    }
}