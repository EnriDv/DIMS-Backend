namespace DIMS_Backend.Features.Materias;

using MediatR;
using DIMS_Backend.Models;

public class CreateMateriaHandler : IRequestHandler<CreateMateriaCommand, int>
{
    private readonly UcbPortalContext _context;
    public CreateMateriaHandler(UcbPortalContext context) => _context = context;

    public async Task<int> Handle(CreateMateriaCommand request, CancellationToken ct)
    {
        var materia = new Materia
        {
            Sigla = request.Sigla,
            Nombre = request.Nombre,
            Creditos = request.Creditos,
            CarreraId = request.CarreraId,
            Semestre = request.Semestre,
            Tipo = request.Tipo,
            Area = request.Area,
            Activa = true
        };

        _context.Materias.Add(materia);
        await _context.SaveChangesAsync(ct);
        return materia.Id;
    }
}