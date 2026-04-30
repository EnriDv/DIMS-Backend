namespace DIMS_Backend.Features.Auth.Login;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DIMS_Backend.Features.Auth.Register;
using DIMS_Backend.Features.Auth.Refresh;
using DIMS_Backend.Features.Auth.Logout;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var result = await _mediator.Send(new LogoutCommand());
        return Ok(result);
    }

    [HttpPost("seed-admin")]
    // [ApiExplorerSettings(IgnoreApi = true)] // Descomenta esto para ocultarlo de Swagger
    public async Task<IActionResult> SeedAdmin([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new { Mensaje = "Usuario administrador creado con éxito", UserId = result });
    }
}
