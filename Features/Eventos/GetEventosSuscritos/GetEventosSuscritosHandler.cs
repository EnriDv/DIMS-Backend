using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DIMS_Backend.Models;
using DIMS_Backend.Features.Eventos.GetEventos;

namespace DIMS_Backend.Features.Eventos.GetEventosSuscritos;

public class GetEventosSuscritosHandler : IRequestHandler<GetEventosSuscritosQuery, List<EventoListDto>>
{
    private readonly UcbPortalContext _context;

    public GetEventosSuscritosHandler(UcbPortalContext context)
    {
        _context = context;
    }

    public async Task<List<EventoListDto>> Handle(GetEventosSuscritosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.EventoSuscripciones
            .Where(s => s.UserId == request.UserId)
            .Include(s => s.Evento)
            .ThenInclude(e => e.Carrera)
            .Select(s => s.Evento)
            .Where(e => e != null && e.FechaEvento >= DateTime.Now && e.Publicado == true);

        return await query
            .OrderBy(e => e.FechaEvento)
            .Select(e => new EventoListDto
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Descripcion = e.Descripcion,
                DescripcionCorta = e.Descripcion != null && e.Descripcion.Length > 100
                                   ? e.Descripcion.Substring(0, 100) + "..."
                                   : e.Descripcion,
                Fecha = e.FechaEvento,
                FechaEvento = e.FechaEvento,
                Ubicacion = e.Lugar,
                Lugar = e.Lugar,
                ImagenUrl = e.ImagenUrl,
                Tipo = e.Tipo,
                CarreraId = e.CarreraId,
                CarreraNombre = e.Carrera != null ? e.Carrera.Nombre : "General",
                Capacidad = e.Capacidad ?? 0,
                Inscritos = e.Inscritos ?? 0,
                Publicado = e.Publicado ?? false,
                CreatedBy = e.CreatedBy,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
            })
            .ToListAsync(cancellationToken);
    }
}
