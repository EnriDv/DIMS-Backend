using MediatR;
using System;
using System.Collections.Generic;
using DIMS_Backend.Features.Eventos.GetEventos;

namespace DIMS_Backend.Features.Eventos.GetEventosSuscritos;

public record GetEventosSuscritosQuery(Guid UserId) : IRequest<List<EventoListDto>>;
