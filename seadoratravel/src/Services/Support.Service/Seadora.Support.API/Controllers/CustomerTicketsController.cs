using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Support.Application.Commands;
using Seadora.Support.Application.Queries;

namespace Seadora.Support.API.Controllers;

[ApiController]
[Route("api/support/tickets")]
[Authorize(Roles = "Customer")]
public class CustomerTicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerTicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (claim == null) throw new UnauthorizedAccessException();
        return Guid.Parse(claim.Value);
    }

    private Guid GetBranchId()
    {
        var claim = User.FindFirst("branch_id");
        if (claim == null) return Guid.Empty; // fallback
        return Guid.TryParse(claim.Value, out var bid) ? bid : Guid.Empty;
    }

    [HttpPost("customer")]
    public async Task<IActionResult> CreateCustomerTicket([FromBody] CreateCustomerTicketRequest request)
    {
        var customerId = GetUserId();
        var branchId = GetBranchId();
        var command = new CreateCustomerTicketCommand(customerId, branchId, request.Subject, request.Description, request.BookingId, request.Category);
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyTickets()
    {
        var customerId = GetUserId();
        var query = new GetCustomerTicketsQuery(customerId);
        var tickets = await _mediator.Send(query);
        return Ok(tickets);
    }

    [HttpPost("customer/{id}/reply")]
    public async Task<IActionResult> ReplyToTicket(Guid id, [FromBody] ReplyToTicketRequest request)
    {
        var customerId = GetUserId();
        // The task says adds message to ticket thread as customer.
        await _mediator.Send(new AddTicketMessageCommand(id, customerId.ToString(), false, request.Body, null));
        return Ok();
    }
}

public record CreateCustomerTicketRequest(string Subject, string Description, Guid? BookingId, string Category);
public record ReplyToTicketRequest(string Body);
