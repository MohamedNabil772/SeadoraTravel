using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Support.Application.Commands;
using Seadora.Support.Application.Queries;

namespace Seadora.Support.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SupportPolicy")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket(CreateTicketCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpPost("{id}/messages")]
    public async Task<IActionResult> AddMessage(Guid id, [FromBody] AddTicketMessageRequest request)
    {
        await _mediator.Send(new AddTicketMessageCommand(id, request.Sender, request.IsFromAgent, request.Body, request.MessageId));
        return Ok();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTicketStatusRequest request)
    {
        await _mediator.Send(new UpdateTicketStatusCommand(id, request.Status));
        return Ok();
    }

    [HttpPut("{id}/assign")]
    public async Task<IActionResult> AssignTicket(Guid id, [FromBody] AssignTicketRequest request)
    {
        await _mediator.Send(new AssignTicketCommand(id, request.AgentId));
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetTickets()
    {
        var tickets = await _mediator.Send(new GetTicketsQuery());
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicket(Guid id)
    {
        var ticket = await _mediator.Send(new GetTicketByIdQuery(id));
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }
}

public record AddTicketMessageRequest(string Sender, bool IsFromAgent, string Body, string? MessageId);
public record UpdateTicketStatusRequest(Seadora.Support.Domain.Enums.TicketStatus Status);
public record AssignTicketRequest(Guid AgentId);
