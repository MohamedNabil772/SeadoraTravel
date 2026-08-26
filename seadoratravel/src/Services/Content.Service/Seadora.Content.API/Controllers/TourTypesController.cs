using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seadora.Common.Messaging.Outbox;
using Seadora.Common.Tenancy;
using Seadora.Content.Application.Common.Interfaces;
using Seadora.Content.Domain.Entities;
using Seadora.Contracts.Enums;
using Seadora.Contracts.Events;

namespace Seadora.Content.API.Controllers;

[ApiController]
[Route("api/v1/tour-types")]
[Route("api/tour-types")]
public class TourTypesController : ControllerBase
{
    private readonly IContentDbContext _context;
    private readonly IOutboxWriter _outbox;
    private readonly ICurrentBranch _currentBranch;

    public TourTypesController(IContentDbContext context, IOutboxWriter outbox, ICurrentBranch currentBranch)
    {
        _context = context;
        _outbox = outbox;
        _currentBranch = currentBranch;
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
        if (InvalidCapacityRange(request.DefaultMinCapacity, request.DefaultMaxCapacity))
        {
            return BadRequest(new { message = "DefaultMaxCapacity must be greater than or equal to DefaultMinCapacity." });
        }

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

        ApplyPolicy(entity, request);

        if (string.IsNullOrWhiteSpace(entity.Code))
        {
            entity.Code = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        }

        _context.TourTypes.Add(entity);
        EnqueuePolicyChanged(entity);
        await _context.SaveChangesAsync(HttpContext.RequestAborted);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTourTypeRequest request)
    {
        var entity = await _context.TourTypes.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return NotFound();

        var newMin = request.DefaultMinCapacity ?? entity.DefaultMinCapacity;
        var newMax = request.DefaultMaxCapacity ?? entity.DefaultMaxCapacity;
        if (InvalidCapacityRange(newMin, newMax))
        {
            return BadRequest(new { message = "DefaultMaxCapacity must be greater than or equal to DefaultMinCapacity." });
        }

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

        ApplyPolicy(entity, request);

        EnqueuePolicyChanged(entity);
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

    private static bool InvalidCapacityRange(int? min, int? max) =>
        min.HasValue && max.HasValue && max.Value < min.Value;

    // ponytail: always enqueue on create/update instead of diffing changed fields -- the consumer
    // upserts idempotently, so a redundant event is cheaper than change tracking.
    private void EnqueuePolicyChanged(TourType entity) =>
        _outbox.Enqueue(new TourTypePolicyChanged
        {
            TourTypeId = entity.Id,
            Code = entity.Code,
            AllocationModel = entity.AllocationModel,
            DefaultMinCapacity = entity.DefaultMinCapacity,
            DefaultMaxCapacity = entity.DefaultMaxCapacity,
            RequiresGuestDetails = entity.RequiresGuestDetails,
            RequiresPassport = entity.RequiresPassport,
            PayLaterAllowed = entity.PayLaterAllowed,
            BranchId = _currentBranch.BranchId
        });

    // ponytail: nulls mean "leave as-is" so partial updates never wipe policy values.
    private static void ApplyPolicy(TourType entity, TourTypePolicyFields p)
    {
        if (p.AllocationModel.HasValue) entity.AllocationModel = p.AllocationModel.Value;
        if (p.DefaultMinCapacity.HasValue) entity.DefaultMinCapacity = p.DefaultMinCapacity.Value;
        if (p.DefaultMaxCapacity.HasValue) entity.DefaultMaxCapacity = p.DefaultMaxCapacity.Value;
        if (p.RequiresGuestDetails.HasValue) entity.RequiresGuestDetails = p.RequiresGuestDetails.Value;
        if (p.RequiresPassport.HasValue) entity.RequiresPassport = p.RequiresPassport.Value;
        if (p.PayLaterAllowed.HasValue) entity.PayLaterAllowed = p.PayLaterAllowed.Value;
    }
}

public class TourTypePolicyFields
{
    public AllocationModel? AllocationModel { get; set; }
    public int? DefaultMinCapacity { get; set; }
    public int? DefaultMaxCapacity { get; set; }
    public bool? RequiresGuestDetails { get; set; }
    public bool? RequiresPassport { get; set; }
    public bool? PayLaterAllowed { get; set; }
}

public class CreateTourTypeRequest : TourTypePolicyFields
{
    public string? Code { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; } = true;
    public Dictionary<string, string>? Names { get; set; }
    public Dictionary<string, string>? Descriptions { get; set; }
}

public class UpdateTourTypeRequest : TourTypePolicyFields
{
    public string? Code { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, string>? Names { get; set; }
    public Dictionary<string, string>? Descriptions { get; set; }
}
