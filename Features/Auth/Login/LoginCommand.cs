namespace DIMS_Backend.Features.Auth.Login;

using MediatR;
using DIMS_Backend.Common;

public class LoginCommand : IRequest<Result<AuthResultDto>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}