using System;
using System.Collections.Generic;

namespace ReconNess.Domain.Entities;

public class Category : BaseEntity, IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Agent> Agents { get; set; }
}
