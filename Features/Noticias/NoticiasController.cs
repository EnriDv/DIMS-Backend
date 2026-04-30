
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DIMS_Backend.Features.Noticias.GetNoticias;
using DIMS_Backend.Features.Noticias.CreateNoticia;

namespace DIMS_Backend.Features.Noticias;

[ApiController]
[Route("api/[controller]")]
public class NoticiasController : ControllerBase
{
    private readonly IMediator _mediator;
    public NoticiasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int? carreraId)
        => Ok(await _mediator.Send(new GetNoticiasQuery(carreraId)));

    [HttpGet("admin")]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> GetAllAdmin([FromQuery] int? carreraId)
        => Ok(await _mediator.Send(new GetNoticiasQuery(carreraId, IncludeUnpublished: true)));

    [HttpPost]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> Create([FromBody] CreateNoticiaCommand command)
    {
        if (string.IsNullOrEmpty(command.Titulo))
            return BadRequest("El título es obligatorio");

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            command.CreatedBy = userId;
        }

        var id = await _mediator.Send(command);

        return Ok(new { id, message = "Noticia creada exitosamente" });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateNoticiaCommand command)
    {
        command.Id = id;

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "ID de usuario no válido en el token" });
        }

        command.RequestUserId = userId;
        command.RequestUserRole = role;

        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,docente")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)!.Value;

        var result = await _mediator.Send(new DeleteNoticiaCommand(id, userId, role));
        return result ? NoContent() : NotFound();
    }
}