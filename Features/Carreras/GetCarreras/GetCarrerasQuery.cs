namespace DIMS_Backend.Features.Carreras.GetCarreras;

using MediatR;

public record GetCarrerasQuery : IRequest<List<CarreraListDto>>;