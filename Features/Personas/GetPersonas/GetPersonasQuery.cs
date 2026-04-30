namespace DIMS_Backend.Features.Personas.GetPersonas;

using MediatR;

public record GetPersonasQuery(int? CarreraId) : IRequest<List<PersonaListDto>>;