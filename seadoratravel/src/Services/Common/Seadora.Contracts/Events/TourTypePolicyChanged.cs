using Seadora.Contracts.Enums;
using Seadora.Contracts.Messaging;

namespace Seadora.Contracts.Events;

public record TourTypePolicyChanged : IntegrationEvent
{
    public Guid TourTypeId { get; init; }
    public string Code { get; init; } = string.Empty;
    public AllocationModel AllocationModel { get; init; }
    public int? DefaultMinCapacity { get; init; }
    public int? DefaultMaxCapacity { get; init; }
    public bool RequiresGuestDetails { get; init; }
    public bool RequiresPassport { get; init; }
    public bool PayLaterAllowed { get; init; }
    public Guid BranchId { get; init; }
}
