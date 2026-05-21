namespace DIMS_Backend.Features.Eventos.SuscribirEvento;

using MediatR;

public record SuscribirEventoCommand(int EventoId, Guid UsuarioId) : IRequest<bool>;