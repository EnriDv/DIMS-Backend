namespace DIMS_Backend.Features.Eventos.SuscribirEvento;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;

public class SuscribirEventoHandler : IRequestHandler<SuscribirEventoCommand, bool>
{
    private readonly UcbPortalContext _context;

    public SuscribirEventoHandler(UcbPortalContext context) => _context = context;

    public async Task<bool> Handle(SuscribirEventoCommand request, CancellationToken cancellationToken)
    {
        var evento = await _context.Eventos.FindAsync(new object[] { request.EventoId }, cancellationToken);

        if (evento == null) return false;

        var yaSuscrito = await _context.EventoSuscripciones
            .AnyAsync(s => s.EventoId == request.EventoId && s.UserId == request.UsuarioId, cancellationToken);

        if (yaSuscrito) return false;

        if (evento.Inscritos >= evento.Capacidad) return false;

        evento.Inscritos++;

        _context.EventoSuscripciones.Add(new EventoSuscripcione
        {
            EventoId = request.EventoId,
            UserId = request.UsuarioId,
        });

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}