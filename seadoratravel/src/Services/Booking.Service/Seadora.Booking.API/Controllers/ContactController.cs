using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Booking.Application.Contact.Commands.CreateContactInquiry;
using Seadora.Booking.Application.Contact.Commands.ReplyToContactInquiry;
using Seadora.Booking.Application.Contact.Queries.GetContactInquiries;

namespace Seadora.Booking.API.Controllers;

[ApiController]
[Route("api/contact")]
public class ContactController : ControllerBase
{
    private readonly ISender _mediator;

    public ContactController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactInquiryCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return Ok(new { id });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var inquiries = await _mediator.Send(new GetContactInquiriesQuery());
        return Ok(inquiries);
    }

    [HttpPost("{id:guid}/reply")]
    public async Task<IActionResult> Reply(Guid id, [FromBody] ReplyToContactInquiryCommand command)
    {
        if (id != command.Id) return BadRequest(new { error = "Mismatched inquiry ID." });
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
