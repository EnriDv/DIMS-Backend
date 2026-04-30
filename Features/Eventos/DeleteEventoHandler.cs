namespace DIMS_Backend.Features.Eventos;

using DIMS_Backend.Models;
using MediatR;

public class DeleteEventoHandler(UcbPortalContext context) : IRequestHandler<DeleteEventoCommand, bool>
{
    public async Task<bool> Handle(DeleteEventoCommand request, CancellationToken cancellationToken)
    {
        var evento = await context.Eventos.FindAsync(request.Id);
        if (evento == null)
        {
            return false;
        }

        evento.Publicado = false;
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
