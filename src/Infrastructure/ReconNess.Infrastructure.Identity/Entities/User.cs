using Microsoft.AspNetCore.Identity;
using ReconNess.Domain;

namespace ReconNess.Infrastructure.Identity.Entities;

public class User : IdentityUser<Guid>, IEntity
{
    /// <summary>
    /// 
    /// </summary>
    public bool Owner { get; set; } = false;

    /// <summary>
    /// 
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the create at
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the update at
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets if is deleted
    /// </summary>
    public bool Deleted { get; set; }

    public string Image { get; set; }
}
