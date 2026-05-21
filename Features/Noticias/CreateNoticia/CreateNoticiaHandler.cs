namespace DIMS_Backend.Features.Noticias.CreateNoticia;

using MediatR;
using DIMS_Backend.Models;

public class CreateNoticiaHandler : IRequestHandler<CreateNoticiaCommand, int>
{
    private readonly UcbPortalContext _context;
    public CreateNoticiaHandler(UcbPortalContext context) => _context = context;

    private static DateTime ToTimestampWithoutTimeZone(DateTime value)
        => DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

    public async Task<int> Handle(CreateNoticiaCommand request, CancellationToken ct)
    {
        var noticia = new Noticia
        {
            Titulo = request.Titulo,
            Contenido = request.Contenido,
            ImagenUrl = request.ImagenUrl,
            CarreraId = request.CarreraId,
            Destacada = request.Destacada,
            Publicada = true,
            CreatedBy = request.CreatedBy,
            Fecha = ToTimestampWithoutTimeZone(DateTime.UtcNow)
        };

        _context.Noticias.Add(noticia);
        await _context.SaveChangesAsync(ct);
        return noticia.Id;
    }
}