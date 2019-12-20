using System;

namespace ReconNess.Entities
{
    public class Note : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Notes { get; set; }

        public virtual Target Target { get; set; }

        public Guid? TargetId { get; set; }

        public virtual Subdomain Subdomain { get; set; }

        public Guid? SubdomainId { get; set; }
    }
}
