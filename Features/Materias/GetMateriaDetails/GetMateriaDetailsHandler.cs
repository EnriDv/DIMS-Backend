namespace DIMS_Backend.Features.Materias.GetMateriaDetails;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public class GetMateriaDetailsHandler : IRequestHandler<GetMateriaDetailsQuery, MateriaDetailDto>
{
    private readonly UcbPortalContext _context;

    public GetMateriaDetailsHandler(UcbPortalContext context)
    {
        _context = context;
    }

    public async Task<MateriaDetailDto> Handle(GetMateriaDetailsQuery request, CancellationToken cancellationToken)
    {
        var materia = await _context.Materias
            // Incluimos la relación N:N con los docentes habilitados para la materia
            .Include(m => m.Docentes)
            // Incluimos los paralelos y, dentro de cada paralelo, al docente que lo imparte
            .Include(m => m.Paralelos)
                .ThenInclude(p => p.Docente)
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.Activa == true, cancellationToken);

        if (materia == null)
        {
            throw new KeyNotFoundException($"No se encontró la materia con ID {request.Id}.");
        }

        return new MateriaDetailDto
        {
            Id = materia.Id,
            Sigla = materia.Sigla,
            Nombre = materia.Nombre,
            Descripcion = materia.Descripcion ?? "",
            Creditos = materia.Creditos,
            Semestre = materia.Semestre,
            Tipo = materia.Tipo,
            Area = materia.Area ?? "",

            Docentes = materia.Docentes.Select(d => new DocenteResumenDto
            {
                Id = d.Id,
                Nombre = d.Nombre,
                FotoUrl = d.FotoUrl
            }).ToList(),

            Paralelos = materia.Paralelos.Select(p => new ParaleloDto
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Horario = p.Horario,
                Aula = p.Aula,
                DocenteId = p.DocenteId,
                DocenteNombre = p.Docente.Nombre, // Gracias al ThenInclude, esto no es nulo
                Cupo = p.Cupo,
                Inscritos = p.Inscritos ?? 0,
                Disponible = p.Disponible ?? true
            }).OrderBy(p => p.Codigo).ToList()
        };
    }
}