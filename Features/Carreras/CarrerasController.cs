using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DIMS_Backend.Features.Carreras.GetCarreraDetails;
using DIMS_Backend.Features.Carreras.GetCarreras;
using DIMS_Backend.Features.Carreras.GetMallaCurricular;

namespace DIMS_Backend.Features.Carreras;

[ApiController]
[Route("api/[controller]")]
public class CarrerasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CarrerasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetCarrerasQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetCarreraDetailsQuery(id));
        return Ok(result);
    }

    [HttpGet("{id}/malla")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMallaCurricular(int id)
    {
        var result = await _mediator.Send(new GetMallaCurricularQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateCarreraCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCarreraCommand command)
    {
        if (id != command.Id) return BadRequest();
        var result = await _mediator.Send(command);
        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteCarreraCommand(id));
        return result ? NoContent() : NotFound();
    }
}