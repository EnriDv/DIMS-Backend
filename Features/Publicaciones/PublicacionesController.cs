namespace DIMS_Backend.Features.Publicaciones;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DIMS_Backend.Features.Publicaciones.GetPublicaciones;
using DIMS_Backend.Features.Publicaciones.CreatePublicacion;

[ApiController]
[Route("api/[controller]")]
public class PublicacionesController : ControllerBase
{
    private readonly IMediator _mediator;
    public PublicacionesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int? carreraId, [FromQuery] string? tipo)
        => Ok(await _mediator.Send(new GetPublicacionesQuery(carreraId, tipo)));

    [HttpPost]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> Create([FromBody] CreatePublicacionCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id, message = "Publicación académica registrada" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value;

        var result = await _mediator.Send(new DeletePublicacionCommand(id, userId, role));
        return result ? NoContent() : NotFound();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePublicacionCommand command)
    {
        if (id != command.Id) return BadRequest("El ID de la URL no coincide con el del cuerpo.");

        // Extraemos los claims del JWT
        command.RequestUserId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        command.RequestUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value;

        var result = await _mediator.Send(command);

        return result ? NoContent() : NotFound();
    }
}