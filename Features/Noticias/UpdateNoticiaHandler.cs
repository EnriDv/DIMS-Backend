namespace DIMS_Backend.Features.Noticias;

using DIMS_Backend.Models;
using MediatR;

public class UpdateNoticiaHandler(UcbPortalContext context) : IRequestHandler<UpdateNoticiaCommand, bool>
{
    private static DateTime ToTimestampWithoutTimeZone(DateTime value)
        => DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

    public async Task<bool> Handle(UpdateNoticiaCommand request, CancellationToken cancellationToken)
    {
        var noticia = await context.Noticias.FindAsync(request.Id);
        if (noticia == null)
        {
            return false;
        }

        if (request.RequestUserRole != "admin" && noticia.CreatedBy != request.RequestUserId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para editar esta noticia.");
        }

        noticia.Titulo = request.Titulo;
        noticia.Contenido = request.Contenido;
        noticia.ImagenUrl = request.ImagenUrl;
        noticia.CarreraId = request.CarreraId;
        noticia.Destacada = request.Destacada;
        noticia.Publicada = request.Publicada;
        noticia.UpdatedAt = ToTimestampWithoutTimeZone(DateTime.UtcNow);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
