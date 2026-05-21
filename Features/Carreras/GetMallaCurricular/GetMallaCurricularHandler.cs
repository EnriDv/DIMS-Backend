namespace DIMS_Backend.Features.Carreras.GetMallaCurricular;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public class GetMallaCurricularHandler : IRequestHandler<GetMallaCurricularQuery, List<MateriaMallaDto>>
{
    private readonly UcbPortalContext _context;

    public GetMallaCurricularHandler(UcbPortalContext context)
    {
        _context = context;
    }

    public async Task<List<MateriaMallaDto>> Handle(GetMallaCurricularQuery request, CancellationToken cancellationToken)
    {
        // Verificamos rápido si la carrera existe
        var carreraExiste = await _context.Carreras
            .AnyAsync(c => c.Id == request.CarreraId && c.Activa == true, cancellationToken);

        if (!carreraExiste)
        {
            throw new KeyNotFoundException($"No se encontró la carrera con ID {request.CarreraId}.");
        }

        var malla = await _context.Materias
            .Where(m => m.CarreraId == request.CarreraId && m.Activa == true)
            .OrderBy(m => m.Semestre) // Orden vital para el frontend
            .ThenBy(m => m.Nombre)
            .Select(m => new MateriaMallaDto
            {
                Id = m.Id,
                Sigla = m.Sigla,
                Nombre = m.Nombre,
                Creditos = m.Creditos,
                Semestre = m.Semestre,
                Tipo = m.Tipo,
                Area = m.Area
            })
            .ToListAsync(cancellationToken);

        return malla;
    }
}