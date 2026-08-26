namespace Seadora.Content.Domain.Entities;

public class TourType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Localized Name & Description
    public Dictionary<string, string> Names { get; set; } = new();
    public Dictionary<string, string> Descriptions { get; set; } = new();
    
    public string Code { get; set; } = string.Empty; // e.g. "GROUP", "PRIVATE", "VIP", "YACHT", "SHORE_EXCURSION", "MULTI_DAY"
    public string Icon { get; set; } = "⛵";
    public int Order { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Arrangement policy: drives how a booking of this tour type is arranged.
    public Seadora.Contracts.Enums.AllocationModel AllocationModel { get; set; } = Seadora.Contracts.Enums.AllocationModel.Shared;
    public int? DefaultMinCapacity { get; set; }
    public int? DefaultMaxCapacity { get; set; }
    public bool RequiresGuestDetails { get; set; }
    public bool RequiresPassport { get; set; }
    public bool PayLaterAllowed { get; set; } = true;
    
    public ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
