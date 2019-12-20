using System;

namespace ReconNess.Entities
{
    public class Service : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Port { get; set; }

        public virtual Subdomain Subdomain { get; set; }
    }
}
