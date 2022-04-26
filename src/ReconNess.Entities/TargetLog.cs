using System;

namespace ReconNess.Entities
{
    public class TargetLog : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Log { get; set; }

        public virtual Target Target { get; set; }
    }
}
