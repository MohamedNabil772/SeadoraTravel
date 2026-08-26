using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/v1/tour-types")]
[Route("api/tour-types")]
public class TourTypesController : ControllerBase
{
    private readonly IContentDbContext _context;

    public TourTypesController(IContentDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] bool includeInactive = false)
    {
        var query = _context.TourTypes.AsNoTracking().AsQueryable();
        if (!includeInactive)
        {
            query = query.Where(t => t.IsActive);
        }

        var types = await query
            .OrderBy(t => t.Order)
            .Select(t => new
            {
                t.Id,
                t.Code,
                t.Icon,
                t.Order,
                t.IsActive,
                t.Names,
                t.Descriptions,
                TourCount = _context.Tours.Count(tour => tour.TourTypeId == t.Id)
            })
            .ToListAsync();

        return Ok(types);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var type = await _context.TourTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (type == null) return NotFound();
        return Ok(type);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTourTypeRequest request)
    {
        var entity = new TourType
        {
            Id = Guid.NewGuid(),
            Code = request.Code?.Trim().ToUpperInvariant() ?? string.Empty,
            Icon = string.IsNullOrWhiteSpace(request.Icon) ? "⛵" : request.Icon.Trim(),
            Order = request.Order,
            IsActive = request.IsActive,
            Names = request.Names ?? new Dictionary<string, string>(),
            Descriptions = request.Descriptions ?? new Dictionary<string, string>()
        };

        if (string.IsNullOrWhiteSpace(entity.Code))
        {
            entity.Code = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        }

        _context.TourTypes.Add(entity);
        await _context.SaveChangesAsync(HttpContext.RequestAborted);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTourTypeRequest request)
    {
        var entity = await _context.TourTypes.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            entity.Code = request.Code.Trim().ToUpperInvariant();
        }
        if (!string.IsNullOrWhiteSpace(request.Icon))
        {
            entity.Icon = request.Icon.Trim();
        }
        entity.Order = request.Order;
        entity.IsActive = request.IsActive;
        if (request.Names != null) entity.Names = request.Names;
        if (request.Descriptions != null) entity.Descriptions = request.Descriptions;

        await _context.SaveChangesAsync(HttpContext.RequestAborted);
        return Ok(entity);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.TourTypes.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return NotFound();

        _context.TourTypes.Remove(entity);
        await _context.SaveChangesAsync(HttpContext.RequestAborted);
        return NoContent();
    }
}

public class CreateTourTypeRequest
{
    public string? Code { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
    public Dictionary<string, string>? Names { get; set; }
    public Dictionary<string, string>? Descriptions { get; set; }
}

public class UpdateTourTypeRequest
{
    public string? Code { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, string>? Names { get; set; }
    public Dictionary<string, string>? Descriptions { get; set; }
}
