using Microsoft.AspNetCore.Identity;

namespace Seadora.Identity.Domain.Entities;

public class Role : IdentityRole<string>
{
    public Role()
    {
        Id = Guid.NewGuid().ToString();
    }
    
    public List<User> Users { get; set; } = new();
}
