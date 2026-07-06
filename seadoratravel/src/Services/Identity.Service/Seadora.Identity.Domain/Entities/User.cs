using Microsoft.AspNetCore.Identity;

namespace Seadora.Identity.Domain.Entities;

public class User : IdentityUser<string>
{
    public User()
    {
        Id = Guid.NewGuid().ToString();
    }
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<Role> Roles { get; set; } = new();
}
