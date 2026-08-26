namespace Seadora.Identity.Domain.Entities;

public class Permission
{
    public string Id { get; set; } = string.Empty; // e.g., "Tours.Create"
    public string Module { get; set; } = string.Empty; // e.g., "Tours & Experiences"
    public string Action { get; set; } = string.Empty; // e.g., "Create"
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
