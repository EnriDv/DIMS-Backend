namespace DIMS_Backend.Features.Eventos;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;
using DIMS_Backend.Features.Eventos.GetEventos;
using DIMS_Backend.Features.Eventos.SuscribirEvento;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int? carreraId, [FromQuery] bool proximos = true)
        => Ok(await _mediator.Send(new GetEventosQuery(carreraId, IncludeUnpublished: false, SoloProximos: proximos)));

    [HttpGet("admin")]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> GetAllAdmin([FromQuery] int? carreraId)
        => Ok(await _mediator.Send(new GetEventosQuery(carreraId, IncludeUnpublished: true, SoloProximos: false)));

    [HttpPost("{id}/suscribir")]
    [Authorize]
    public async Task<IActionResult> Suscribir(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value; 

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized(new { message = "ID de usuario no válido en el token" });
        }

        try
        {
            var result = await _mediator.Send(new SuscribirEventoCommand(id, userId));
            return result ? Ok(new { message = "Suscripción exitosa" })
                          : BadRequest("No hay cupos disponibles o ya estás inscrito");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno en el Handler", details = ex.Message });
            
        }
    }

    [HttpPost]
    [Authorize(Roles = "admin,docente")] 
    public async Task<IActionResult> Create([FromBody] CreateEventoCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized(new { message = "ID de usuario no válido en el token" });
        }

        var commandWithAuthor = command with { CreatedBy = userId };

        try
        {
            var result = await _mediator.Send(commandWithAuthor);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventoCommand command)
    {
        command.Id = id;

        try
        {
            var result = await _mediator.Send(command);
            return result ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteEventoCommand(id));
        return result ? NoContent() : NotFound();
    }
}