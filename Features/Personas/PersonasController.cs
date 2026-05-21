namespace DIMS_Backend.Features.Personas;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DIMS_Backend.Features.Personas.GetPersonas;
using DIMS_Backend.Features.Personas.GetPersonaDetails;

[ApiController]
[Route("api/[controller]")]
public class PersonasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/personas?carreraId=1
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int? carreraId)
    {
        var result = await _mediator.Send(new GetPersonasQuery(carreraId));
        return Ok(result);
    }

    // GET: api/personas/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetPersonaDetailsQuery(id));
        return Ok(result);
    }
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreatePersonaCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }
}