using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Booking.Application.Bookings.Commands.CreateBooking;
using Seadora.Booking.Application.Bookings.Commands.UpdateBookingStatus;
using Seadora.Booking.Application.Bookings.Queries;

namespace Seadora.Booking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly ISender _mediator;

    public BookingsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
    {
        try
        {
            var bookingId = await _mediator.Send(command);
            return Ok(bookingId);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? tourId, [FromQuery] string? status)
    {
        var bookings = await _mediator.Send(new GetAllBookingsQuery(tourId, status));
        return Ok(bookings);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var booking = await _mediator.Send(new GetBookingByIdQuery(id));
            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBookingStatusCommand command)
    {
        if (id != command.Id) return BadRequest(new { error = "Mismatched booking ID." });
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
