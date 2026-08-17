using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Languages;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Policy = "AdminPolicy")]
public class LanguagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LanguagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] bool includeInactive = false)
    {
        return Ok(await _mediator.Send(new GetLanguagesQuery(includeInactive)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLanguageCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateLanguageCommand command)
    {
        if (id != command.Id) return BadRequest();
        return Ok(await _mediator.Send(command));
    }

    [HttpGet("{languageCode}/translations")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTranslations(string languageCode)
    {
        return Ok(await _mediator.Send(new GetTranslationsQuery(languageCode)));
    }

    [HttpPut("{languageCode}/translations")]
    public async Task<IActionResult> BulkUpdateTranslations(string languageCode, BulkUpdateTranslationsCommand command)
    {
        if (languageCode != command.LanguageCode) return BadRequest();
        return Ok(await _mediator.Send(command));
    }
}
