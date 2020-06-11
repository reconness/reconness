using System;

namespace ReconNess.Entities
{
    public class Notification : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string Payload { get; set; }
    }
}
