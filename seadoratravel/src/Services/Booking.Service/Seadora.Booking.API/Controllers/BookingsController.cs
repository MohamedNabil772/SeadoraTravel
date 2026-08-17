using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Seadora.Booking.Application.Bookings.Commands.CreateBooking;
using Seadora.Booking.Application.Bookings.Commands.UpdateBookingStatus;
using Seadora.Booking.Application.Bookings.Commands.UpdateBookingAttendance;
using Seadora.Booking.Application.Bookings.Commands.UpdateBookingPayment;
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
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? tourId, 
        [FromQuery] Seadora.Booking.Domain.Enums.BookingStatus? status,
        [FromQuery] string? sortColumn,
        [FromQuery] string? sortOrder,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var bookings = await _mediator.Send(new GetAllBookingsQuery(tourId, status, sortColumn, sortOrder, pageNumber, pageSize));
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

    [HttpGet("{tourId:guid}/availability")]
    public async Task<IActionResult> GetTourAvailability(Guid tourId, [FromQuery] DateTime date)
    {
        var bookedGuests = await _mediator.Send(new GetTourAvailabilityQuery(tourId, date));
        return Ok(new { tourId, date, bookedGuests });
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

    [HttpPut("{id:guid}/attendance")]
    public async Task<IActionResult> UpdateAttendance(Guid id, [FromBody] UpdateBookingAttendanceCommand command)
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

    [HttpPut("{id:guid}/payment")]
    public async Task<IActionResult> UpdatePayment(Guid id, [FromBody] UpdateBookingPaymentCommand command)
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
