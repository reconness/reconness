using System;

namespace ReconNess.Entities
{
    public class AgentNotification : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string SubdomainPayload { get; set; }

        public string IpAddressPayload { get; set; }

        public string IsAlivePayload { get; set; }

        public string HasHttpOpenPayload { get; set; }

        public string TakeoverPayload { get; set; }

        public string DirectoryPayload { get; set; }

        public string ServicePayload { get; set; }

        public string NotePayload { get; set; }

        public Guid AgentRef { get; set; }

        public virtual Agent Agent { get; set; }
    }
}
