namespace DIMS_Backend.Features.Carreras.GetMallaCurricular;

using MediatR;

public record GetMallaCurricularQuery(int CarreraId) : IRequest<List<MateriaMallaDto>>;