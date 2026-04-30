namespace DIMS_Backend.Features.Carreras.GetCarreraDetails;

using MediatR;

public record GetCarreraDetailsQuery(int Id) : IRequest<CarreraDetailDto>;