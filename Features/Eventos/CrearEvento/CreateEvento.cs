// DIMS_Backend/Features/Eventos/CreateEvento/CreateEventoCommand.cs
using MediatR;

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
) : IRequest<int>; // Retornamos el ID del nuevo evento