using Seadora.Contracts.Enums;
using Seadora.Contracts.Messaging;

namespace Seadora.Contracts.Events;

public record TourUpdated : IntegrationEvent
{
    public Guid TourId { get; init; }
    public Guid BranchId { get; init; }
    public string? TourTypeCode { get; init; }
    public AllocationModel AllocationModel { get; init; }
    public int MinCapacity { get; init; }
    public int MaxCapacity { get; init; }
    public bool RequiresGuestDetails { get; init; }
    public bool RequiresPassport { get; init; }
    public bool PayLaterAllowed { get; init; }
    public decimal PriceFrom { get; init; }
    public string Currency { get; init; } = "EUR";
}
