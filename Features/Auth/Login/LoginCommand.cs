namespace DIMS_Backend.Features.Auth.Login;

using MediatR;

public class LoginCommand : IRequest<AuthResultDto>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}