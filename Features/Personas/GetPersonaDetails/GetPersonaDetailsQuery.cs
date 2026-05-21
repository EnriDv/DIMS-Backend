namespace DIMS_Backend.Features.Personas.GetPersonaDetails;

using MediatR;

public record GetPersonaDetailsQuery(int Id) : IRequest<PersonaDetailDto>;
