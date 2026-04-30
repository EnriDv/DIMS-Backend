namespace DIMS_Backend.Features.Materias;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DIMS_Backend.Features.Materias.GetMateriaDetails;

[ApiController]
[Route("api/[controller]")]
public class MateriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public MateriasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/materias/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetMateriaDetailsQuery(id));
        return Ok(result);
    }
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateMateriaCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteMateriaCommand(id));
        return result ? NoContent() : NotFound();
    }
}