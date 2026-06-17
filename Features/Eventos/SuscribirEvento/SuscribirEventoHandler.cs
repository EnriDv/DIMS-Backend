namespace DIMS_Backend.Features.Eventos.SuscribirEvento;

using MediatR;
using Microsoft.EntityFrameworkCore;
using DIMS_Backend.Models;
using DIMS_Backend.Infrastructure.BackgroundServices;
using System.Text.Json;
using System;

public class SuscribirEventoHandler : IRequestHandler<SuscribirEventoCommand, bool>
{
    private readonly UcbPortalContext _context;
    private readonly S3BackgroundQueue _s3Queue;

    public SuscribirEventoHandler(UcbPortalContext context, S3BackgroundQueue s3Queue)
    {
        _context = context;
        _s3Queue = s3Queue;
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

            // Fetch user info to include in notification JSON
            var user = await _context.Users.FindAsync(new object[] { request.UsuarioId }, cancellationToken);
            if (user != null)
            {
                var auditPayload = new
                {
                    Email = user.Email,
                    NombreEstudiante = user.Nombre,
                    EventoId = evento.Id,
                    TituloEvento = evento.Titulo,
                    FechaSuscripcion = DateTime.UtcNow
                };

                var job = new S3UploadJob(
                    Folder: "subscriptions",
                    FileName: $"sub-{evento.Id}-{user.Id}-{Guid.NewGuid()}.json",
                    ContentBody: JsonSerializer.Serialize(auditPayload),
                    ContentType: "application/json"
                );

                _s3Queue.Writer.TryWrite(job);
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
