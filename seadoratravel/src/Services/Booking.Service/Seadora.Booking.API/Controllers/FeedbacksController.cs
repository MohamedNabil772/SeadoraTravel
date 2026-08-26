using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seadora.Booking.Application.Feedbacks.Commands.CreateFeedback;
using Seadora.Booking.Application.Feedbacks.Commands.UpdateFeedbackVisibility;
using Seadora.Booking.Application.Feedbacks.Queries.GetFeedbacks;

namespace Seadora.Booking.API.Controllers;

[ApiController]
[Route("api/feedbacks")]
public class FeedbacksController : ControllerBase
{
    private readonly ISender _mediator;

    public FeedbacksController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackCommand command)
    {
        try
        {
            var feedback = await _mediator.Send(command);
            return Ok(feedback);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] Guid? tourId, [FromQuery] bool includeHidden = false)
    {
        var query = new GetFeedbacksQuery(tourId, includeHidden);
        var feedbacks = await _mediator.Send(query);
        return Ok(feedbacks);
    }

    [HttpPut("{id:guid}/visibility")]
    public async Task<IActionResult> UpdateVisibility(Guid id, [FromBody] UpdateFeedbackVisibilityCommand command)
    {
        if (id != command.Id) return BadRequest(new { error = "Mismatched feedback ID." });
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
