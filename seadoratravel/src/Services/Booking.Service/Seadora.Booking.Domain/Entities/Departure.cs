using Seadora.Contracts.Enums;

namespace Seadora.Booking.Domain.Entities;

public class Departure
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public Guid TourId { get; set; }
    public DateTime StartUtc { get; set; }
    public string TimeSlot { get; set; } = "";
    public int Capacity { get; set; }
    public AllocationModel AllocationModel { get; set; }

    // ponytail: PostgreSQL xmin system column as the concurrency token - no real column, no bookkeeping.
    public uint Version { get; set; }
}
