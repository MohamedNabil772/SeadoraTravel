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
        return Ok(customer);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var id = GetUserId();
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        if (customer == null) return NotFound();
        
        customer.Phone = request.Phone;
        customer.Nationality = request.Nationality;
        customer.PassportNumber = request.PassportNumber;
        customer.UpdatedUtc = DateTime.UtcNow;
        
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(default);
        return Ok(customer);
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
    public string? Phone { get; set; }
    public string? Nationality { get; set; }
    public string? PassportNumber { get; set; }
}
