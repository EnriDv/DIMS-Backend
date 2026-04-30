namespace DIMS_Backend.Features.Publicaciones.CreatePublicacion;

using MediatR;
using DIMS_Backend.Models;

public class CreatePublicacionHandler : IRequestHandler<CreatePublicacionCommand, int>
{
    private readonly UcbPortalContext _context;
    public CreatePublicacionHandler(UcbPortalContext context) => _context = context;

    public async Task<int> Handle(CreatePublicacionCommand request, CancellationToken ct)
    {
        var publicacion = new Publicacione
        {
            Titulo = request.Titulo,
            Autor = request.Autor,
            Resumen = request.Resumen,
            ArchivoUrl = request.ArchivoUrl,
            CarreraId = request.CarreraId,
            Tipo = request.Tipo,
            FechaPublicacion = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        _context.Publicaciones.Add(publicacion);
        await _context.SaveChangesAsync(ct);
        return publicacion.Id;
    }
}