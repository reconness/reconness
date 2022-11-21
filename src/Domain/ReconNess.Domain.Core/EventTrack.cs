using ReconNess.Entities.Enum;
using System;

namespace ReconNess.Entities
{
    public class EventTrack : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public string Username { get; set; }

        public EventTrackStatus Status { get; set; } = EventTrackStatus.SUCCESS;

        public bool Read { get; set; }

        public virtual Agent Agent { get; set; }

        public virtual Target Target { get; set; }

        public virtual RootDomain RootDomain { get; set; }

        public virtual Subdomain Subdomain { get; set; }
    }
}
