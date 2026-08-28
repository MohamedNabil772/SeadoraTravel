using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seadora.Customer.Application.Common.Interfaces;

namespace Seadora.Customer.API.Controllers;

[ApiController]
[Route("api/customer/portal")]
[Authorize(Roles = "Customer")]
public class CustomerPortalController : ControllerBase
{
    private readonly ICustomerDbContext _context;

    public CustomerPortalController(ICustomerDbContext context)
    {
        _context = context;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        if (claim == null) throw new UnauthorizedAccessException();
        return Guid.Parse(claim.Value);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var id = GetUserId();
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();
        
        return Ok(new {
            id = customer.Id,
            email = customer.Email,
            fullName = customer.FullName,
            phoneNumber = customer.Phone,
            avatarUrl = customer.AvatarUrl,
            preferredLanguage = customer.PreferredLanguage,
            dietaryRequirements = customer.DietaryRequirements
        });
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var id = GetUserId();
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();
        
        if (request.FullName != null) customer.FullName = request.FullName;
        customer.Phone = request.PhoneNumber;
        customer.AvatarUrl = request.AvatarUrl;
        if (request.PreferredLanguage != null) customer.PreferredLanguage = request.PreferredLanguage;
        customer.DietaryRequirements = request.DietaryRequirements;
        customer.UpdatedUtc = DateTime.UtcNow;
        
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(default);
        
        return Ok(new {
            id = customer.Id,
            email = customer.Email,
            fullName = customer.FullName,
            phoneNumber = customer.Phone,
            avatarUrl = customer.AvatarUrl,
            preferredLanguage = customer.PreferredLanguage,
            dietaryRequirements = customer.DietaryRequirements
        });
    }

    [HttpGet("bookings/{id}/voucher")]
    public async Task<IActionResult> GetVoucher(Guid id)
    {
        var userId = GetUserId();
        var booking = await _context.BookingHistory.FirstOrDefaultAsync(b => b.BookingId == id && b.CustomerId == userId);
        if (booking == null) return NotFound();

        // Stubbing voucher payload as full data would come from Booking/Tour service
        return Ok(new {
            bookingId = booking.BookingId,
            bookingReference = $"BKG-{booking.BookingId.ToString().Substring(0, 8).ToUpper()}",
            tourTitle = "Seadora Exclusive Tour",
            departureDate = booking.TourDate ?? DateTime.UtcNow.AddDays(7),
            returnDate = (booking.TourDate ?? DateTime.UtcNow.AddDays(7)).AddDays(3),
            destination = "Maldives",
            pickupLocation = "Main Lobby, Seadora Resort",
            roomType = "Ocean Villa",
            guestsCount = 2,
            passengers = new[] {
                new { name = "John Doe", type = "Adult" },
                new { name = "Jane Doe", type = "Adult" }
            },
            amountPaid = booking.Amount,
            currency = booking.Currency,
            isPaid = true,
            qrCodeData = $"VOUCHER:{booking.BookingId}",
            emergencyPhone = "+1-800-SEADORA",
            generatedAtUtc = DateTime.UtcNow
        });
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings()
    {
        var id = GetUserId();
        var bookings = await _context.BookingHistory.Where(b => b.CustomerId == id).ToListAsync();
        return Ok(bookings);
    }

    [HttpGet("documents")]
    public async Task<IActionResult> GetDocuments()
    {
        var id = GetUserId();
        var docs = await _context.CustomerDocuments.Where(d => d.CustomerId == id).ToListAsync();
        return Ok(docs);
    }
}

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? DietaryRequirements { get; set; }
}
