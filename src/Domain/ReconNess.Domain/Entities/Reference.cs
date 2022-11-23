using System;

namespace ReconNess.Domain.Entities;

public class Reference : BaseEntity, IEntity
{
    public Guid Id { get; set; }

    public string Url { get; set; }

    public string Categories { get; set; }
}
