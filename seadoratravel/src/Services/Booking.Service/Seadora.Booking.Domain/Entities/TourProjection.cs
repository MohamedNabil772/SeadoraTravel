using Seadora.Contracts.Enums;

namespace Seadora.Booking.Domain.Entities;

public class TourProjection
{
    public Guid TourId { get; set; }
    public Guid BranchId { get; set; }
    public string? TourTypeCode { get; set; }
    public AllocationModel AllocationModel { get; set; }
    public int MinCapacity { get; set; }
    public int MaxCapacity { get; set; }
    public bool RequiresGuestDetails { get; set; }
    public bool RequiresPassport { get; set; }
    public bool PayLaterAllowed { get; set; }
    public decimal PriceFrom { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateTime UpdatedUtc { get; set; }
}
