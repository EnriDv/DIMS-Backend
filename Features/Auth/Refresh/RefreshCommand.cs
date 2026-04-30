namespace DIMS_Backend.Features.Auth.Refresh;

using MediatR;
using DIMS_Backend.Features.Auth.Login;

public class RefreshCommand : IRequest<RefreshTokenResultDto>
{
    public string RefreshToken { get; set; } = string.Empty;
}
