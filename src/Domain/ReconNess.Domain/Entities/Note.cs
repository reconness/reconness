using System;

namespace ReconNess.Domain.Entities;

public class Note : BaseEntity, IEntity
{
    public Guid Id { get; set; }

    public string CreatedBy { get; set; }

    public string Comment { get; set; }

    public virtual Target Target { get; set; }

    public virtual RootDomain RootDomain { get; set; }

    public virtual Subdomain Subdomain { get; set; }
}
