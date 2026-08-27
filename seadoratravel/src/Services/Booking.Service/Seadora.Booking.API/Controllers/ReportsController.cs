using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seadora.Booking.Application.Common.Interfaces;

namespace Seadora.Booking.API.Controllers;

// ponytail: Superseded by the Finance service (Seadora.Finance) double-entry ledger reports.
// Retained (deprecated) so existing admin ReportsView keeps working during the transition; do not extend.
[Obsolete("Legacy operational reporting. Use the Finance service (api/finance/api/reports & /dashboard) instead.")]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IBookingDbContext _context;
    private readonly IHttpClientFactory _clientFactory;

    public ReportsController(IBookingDbContext context, IHttpClientFactory clientFactory)
    {
        _context = context;
        _clientFactory = clientFactory;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var tours = await GetToursAsync();
        var bookings = await _context.Bookings.ToListAsync();

        var confirmedBookings = bookings.Where(b => b.Status != Seadora.Booking.Domain.Enums.BookingStatus.Cancelled).ToList();
        
        decimal totalRevenue = 0;
        decimal totalSupplierCost = 0;

        foreach (var booking in confirmedBookings)
        {
            var tour = tours.FirstOrDefault(t => t.Id == booking.TourId);
            if (tour != null)
            {
                totalRevenue += tour.Price;
                totalSupplierCost += tour.Price * (tour.SupplierPercentage / 100);
            }
        }

        decimal totalPlatformEarnings = totalRevenue - totalSupplierCost;

        // Group Daily (last 7 days)
        var daily = new List<object>();
        for (int i = 6; i >= 0; i--)
        {
            var date = DateTime.UtcNow.Date.AddDays(-i);
            var dayBookings = confirmedBookings.Where(b => b.BookingDate.Date == date).ToList();
            decimal dayRev = 0;
            decimal dayCost = 0;
            foreach (var b in dayBookings)
            {
                var tour = tours.FirstOrDefault(t => t.Id == b.TourId);
                if (tour != null)
                {
                    dayRev += tour.Price;
                    dayCost += tour.Price * (tour.SupplierPercentage / 100);
                }
            }
            daily.Add(new { Date = date.ToString("yyyy-MM-dd"), Revenue = dayRev, Earnings = dayRev - dayCost });
        }

        // Group Weekly (last 4 weeks)
        var weekly = new List<object>();
        for (int i = 3; i >= 0; i--)
        {
            var dateStart = DateTime.UtcNow.Date.AddDays(-i * 7 - 6);
            var dateEnd = DateTime.UtcNow.Date.AddDays(-i * 7);
            var weekBookings = confirmedBookings.Where(b => b.BookingDate.Date >= dateStart && b.BookingDate.Date <= dateEnd).ToList();
            decimal weekRev = 0;
            decimal weekCost = 0;
            foreach (var b in weekBookings)
            {
                var tour = tours.FirstOrDefault(t => t.Id == b.TourId);
                if (tour != null)
                {
                    weekRev += tour.Price;
                    weekCost += tour.Price * (tour.SupplierPercentage / 100);
                }
            }
            weekly.Add(new { Week = $"Week -{i}", Revenue = weekRev, Earnings = weekRev - weekCost });
        }

        // Group Monthly (last 6 months)
        var monthly = new List<object>();
        for (int i = 5; i >= 0; i--)
        {
            var date = DateTime.UtcNow.AddMonths(-i);
            var monthBookings = confirmedBookings.Where(b => b.BookingDate.Month == date.Month && b.BookingDate.Year == date.Year).ToList();
            decimal monthRev = 0;
            decimal monthCost = 0;
            foreach (var b in monthBookings)
            {
                var tour = tours.FirstOrDefault(t => t.Id == b.TourId);
                if (tour != null)
                {
                    monthRev += tour.Price;
                    monthCost += tour.Price * (tour.SupplierPercentage / 100);
                }
            }
            monthly.Add(new { Month = date.ToString("MMM yyyy"), Revenue = monthRev, Earnings = monthRev - monthCost });
        }

        var recent = bookings.OrderByDescending(b => b.BookingDate).Take(8).Select(b => {
            var tour = tours.FirstOrDefault(t => t.Id == b.TourId);
            return new {
                b.Id,
                b.CustomerName,
                b.CustomerEmail,
                b.BookingDate,
                b.Status,
                TourName = tour != null ? (tour.Names.ContainsKey("en") ? tour.Names["en"] : "Egypt Tour") : "Unknown Tour",
                Price = tour != null ? tour.Price : 0
            };
        });

        return Ok(new {
            totalBookings = bookings.Count,
            totalRevenue,
            totalSupplierCost,
            totalPlatformEarnings,
            daily,
            weekly,
            monthly,
            recentBookings = recent
        });
    }

    [HttpGet("supplier")]
    public async Task<IActionResult> GetSupplierReport([FromQuery] string duration = "all")
    {
        var tours = await GetToursAsync();
        var bookings = await _context.Bookings.ToListAsync();

        var dateFilter = DateTime.MinValue;
        if (duration.ToLower() == "day") dateFilter = DateTime.UtcNow.AddDays(-1);
        else if (duration.ToLower() == "week") dateFilter = DateTime.UtcNow.AddDays(-7);
        else if (duration.ToLower() == "month") dateFilter = DateTime.UtcNow.AddMonths(-1);

        var filteredBookings = bookings
            .Where(b => b.Status != Seadora.Booking.Domain.Enums.BookingStatus.Cancelled && (duration == "all" || b.BookingDate >= dateFilter))
            .ToList();

        // Group tours by supplier
        var supplierReports = new List<object>();

        // Find all distinct suppliers from tours response
        var suppliersDict = new Dictionary<Guid, (string NameEn, string NameAr, string Agreement)>();
        foreach (var tour in tours)
        {
            if (tour.SupplierId.HasValue && !suppliersDict.ContainsKey(tour.SupplierId.Value))
            {
                var nameEn = tour.SupplierNameEn ?? "Unknown Supplier";
                var nameAr = tour.SupplierNameAr ?? "مورد غير معروف";
                var agreement = tour.SupplierAgreement ?? "Weekly";
                suppliersDict.Add(tour.SupplierId.Value, (nameEn, nameAr, agreement));
            }
        }

        foreach (var sKvp in suppliersDict)
        {
            var sId = sKvp.Key;
            var sInfo = sKvp.Value;

            var supplierTours = tours.Where(t => t.SupplierId == sId).Select(t => t.Id).ToList();
            var supplierBookings = filteredBookings.Where(b => supplierTours.Contains(b.TourId)).ToList();

            decimal totalRevenue = 0;
            decimal totalSupplierCost = 0;

            foreach (var b in supplierBookings)
            {
                var tour = tours.FirstOrDefault(t => t.Id == b.TourId);
                if (tour != null)
                {
                    totalRevenue += tour.Price;
                    totalSupplierCost += tour.Price * (tour.SupplierPercentage / 100);
                }
            }

            supplierReports.Add(new {
                supplierId = sId,
                nameEn = sInfo.NameEn,
                nameAr = sInfo.NameAr,
                agreement = sInfo.Agreement,
                bookingCount = supplierBookings.Count,
                totalRevenue,
                totalCost = totalSupplierCost // what Seadora pays to supplier
            });
        }

        return Ok(supplierReports);
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomersPerTrip()
    {
        var tours = await GetToursAsync();
        var bookings = await _context.Bookings.ToListAsync();

        var reports = tours.Select(t => {
            var tourBookings = bookings.Where(b => b.TourId == t.Id).OrderByDescending(b => b.BookingDate).Select(b => new {
                b.Id,
                b.CustomerName,
                b.CustomerEmail,
                b.BookingDate,
                b.Status
            }).ToList();

            return new {
                tourId = t.Id,
                tourNameEn = t.Names.ContainsKey("en") ? t.Names["en"] : "Tour",
                tourNameAr = t.Names.ContainsKey("ar") ? t.Names["ar"] : "",
                price = t.Price,
                supplierName = t.SupplierNameEn ?? "Unknown",
                bookings = tourBookings,
                bookingCount = tourBookings.Count
            };
        }).ToList();

        return Ok(reports);
    }

    [HttpGet("ledger")]
    public async Task<IActionResult> GetLedger()
    {
        var tours = await GetToursAsync();
        var bookings = await _context.Bookings.ToListAsync();

        var ledger = bookings.OrderByDescending(b => b.BookingDate).Select(b => {
            var tour = tours.FirstOrDefault(t => t.Id == b.TourId);
            decimal grossRevenue = tour != null ? tour.Price : 0;
            decimal supplierShare = tour != null ? tour.Price * (tour.SupplierPercentage / 100) : 0;
            decimal platformProfit = grossRevenue - supplierShare;

            return new {
                bookingId = b.Id,
                bookingDate = b.BookingDate,
                customerName = b.CustomerName,
                customerEmail = b.CustomerEmail,
                tourName = tour != null ? (tour.Names.ContainsKey("en") ? tour.Names["en"] : "Egypt Tour") : "Unknown Tour",
                supplierName = tour != null ? (tour.SupplierNameEn ?? "Direct Platform") : "Direct Platform",
                grossRevenue,
                supplierShare,
                platformProfit,
                status = b.Status
            };
        }).ToList();

        return Ok(ledger);
    }

    private async Task<List<TourDto>> GetToursAsync()
    {
        var client = _clientFactory.CreateClient();
        var urls = new[] {
            "http://content-service:8080/api/tours",
            "http://localhost:8000/api/content/api/tours"
        };
        foreach (var url in urls)
        {
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tours = System.Text.Json.JsonSerializer.Deserialize<List<TourDto>>(content, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (tours != null) return tours;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tours from {url}: {ex.Message}");
            }
        }
        return new List<TourDto>();
    }
}

public class TourDto
{
    public Guid Id { get; set; }
    public Dictionary<string, string> Names { get; set; } = new();
    public decimal Price { get; set; }
    public Guid? SupplierId { get; set; }
    public decimal SupplierPercentage { get; set; }
    public int MaxAllocations { get; set; } = 20;
    
    // Dynamic mapping properties helper
    public string? SupplierNameEn => Supplier?.NameEn;
    public string? SupplierNameAr => Supplier?.NameAr;
    public string? SupplierAgreement => Supplier?.PaymentAgreement?.Name;

    public SupplierDto? Supplier { get; set; }
}

public class SupplierDto
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public PaymentAgreementDto? PaymentAgreement { get; set; }
}

public class PaymentAgreementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
