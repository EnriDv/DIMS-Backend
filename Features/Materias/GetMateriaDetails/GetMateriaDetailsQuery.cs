namespace DIMS_Backend.Features.Materias.GetMateriaDetails;

using MediatR;

public record GetMateriaDetailsQuery(int Id) : IRequest<MateriaDetailDto>;