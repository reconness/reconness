namespace ReconNess.Entities
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the create at
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the update at
        /// </summary>
        DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets if is deleted
        /// </summary>
        bool Deleted { get; set; }
    }
}
