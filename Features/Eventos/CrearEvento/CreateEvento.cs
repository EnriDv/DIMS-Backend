namespace DIMS_Backend.Features.Eventos.CrearEvento;

using MediatR;
using System;

public record CreateEventoCommand(
    string Titulo,
    string Descripcion,
    DateTime FechaEvento,
    string Lugar,
    string Tipo,
    int? CarreraId,
    int Capacidad,
    string? ImagenUrl,
    Guid? CreatedBy = null
) : IRequest<int>;
