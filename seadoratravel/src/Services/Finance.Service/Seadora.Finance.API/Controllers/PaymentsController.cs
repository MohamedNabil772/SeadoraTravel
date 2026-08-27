using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seadora.Finance.Application.Common.Interfaces;
using Seadora.Finance.Application.Payments.Commands.RecordPayment;
using Seadora.Finance.Domain.Enums;

namespace Seadora.Finance.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize(Policy = "Finance.ManagePayments")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IFinanceDbContext _db;

    public PaymentsController(ISender mediator, IFinanceDbContext db)
    {
        _mediator = mediator;
        _db = db;
    }

    public record RecordPaymentRequest(decimal Amount, PaymentMethod Method, string? Reference, DateTime? ReceivedUtc);

    [HttpPost("booking/{bookingId:guid}")]
    public async Task<IActionResult> Record(Guid bookingId, [FromBody] RecordPaymentRequest request)
    {
        try
        {
            var createdBy = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var id = await _mediator.Send(new RecordPaymentCommand(
                bookingId, request.Amount, request.Method, request.Reference, request.ReceivedUtc, createdBy));
            return Ok(new { paymentId = id });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("booking/{bookingId:guid}")]
    public async Task<IActionResult> ListForBooking(Guid bookingId, CancellationToken ct)
    {
        var payments = await _db.Payments
            .Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.ReceivedUtc)
            .Select(p => new
            {
                p.Id,
                p.BookingId,
                p.Amount,
                p.Currency,
                Method = p.Method.ToString(),
                p.Reference,
                p.ReceivedUtc,
                p.CreatedBy
            })
            .ToListAsync(ct);
        return Ok(payments);
    }
}
