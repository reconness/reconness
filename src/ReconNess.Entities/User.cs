using Microsoft.AspNetCore.Identity;
using System;

namespace ReconNess.Entities
{
    public class User : IdentityUser<Guid>, IEntity
    {
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
    }
}
