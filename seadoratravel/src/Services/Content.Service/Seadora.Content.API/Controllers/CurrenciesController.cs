using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Content.Application.Currencies;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/v1/currencies")]
[Route("api/currencies")]
public class CurrenciesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CurrenciesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] bool includeInactive = false)
    {
        return Ok(await _mediator.Send(new GetCurrenciesQuery(includeInactive)));
    }

    [HttpPost("sync-rates")]
    public async Task<IActionResult> SyncLiveRates()
    {
        return Ok(await _mediator.Send(new SyncLiveExchangeRatesCommand()));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCurrencyCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateCurrencyCommand command)
    {
        if (id != command.Id) return BadRequest();
        return Ok(await _mediator.Send(command));
    }

    [HttpPatch("{id}/rate")]
    public async Task<IActionResult> UpdateRate(Guid id, UpdateCurrencyRateCommand command)
    {
        if (id != command.Id) return BadRequest();
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("{id}/reset-live-rate")]
    public async Task<IActionResult> ResetLiveRate(Guid id)
    {
        return Ok(await _mediator.Send(new ResetCurrencyToLiveRateCommand(id)));
    }

    [HttpPost("{id}/set-base")]
    public async Task<IActionResult> SetBase(Guid id)
    {
        return Ok(await _mediator.Send(new SetBaseCurrencyCommand(id)));
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(Guid id, ToggleCurrencyActiveCommand command)
    {
        if (id != command.Id) return BadRequest();
        return Ok(await _mediator.Send(command));
    }
}
