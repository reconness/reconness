using System;

namespace ReconNess.Domain.Entities;

public class Directory : BaseEntity, IEntity
{
    public Guid Id { get; set; }

    public string Uri { get; set; }

    public string StatusCode { get; set; }

    public string Size { get; set; }

    public string Method { get; set; }

    public virtual Subdomain Subdomain { get; set; }
}
