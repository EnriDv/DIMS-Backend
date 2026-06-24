namespace DIMS_Backend.Features.Eventos.SuscribirEvento;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Infrastructure.Messaging;
using System.Text.Json;

public class SuscribirEventoHandler : IRequestHandler<SuscribirEventoCommand, bool>
{
    private readonly UcbPortalContext _context;
    private readonly ISqsService _sqsService;

    public SuscribirEventoHandler(UcbPortalContext context, ISqsService sqsService)
    {
        _context = context;
        _sqsService = sqsService;
    }

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

            var user = await _context.Users.FindAsync(new object[] { request.UsuarioId }, cancellationToken);
            if (user != null)
            {
                var payload = new
                {
                    Email = user.Email,
                    NombreEstudiante = user.Nombre,
                    EventoId = evento.Id,
                    TituloEvento = evento.Titulo,
                    FechaSuscripcion = DateTime.UtcNow,
                };

                await _sqsService.SendMessageAsync(JsonSerializer.Serialize(payload), cancellationToken);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
