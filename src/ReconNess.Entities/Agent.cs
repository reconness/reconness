using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class Agent : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Command { get; set; }

        public string Repository { get; set; }

        public string Script { get; set; }

        public DateTime? LastRun { get; set; }

        public string ConfigurationFileName { get; set; }

        public virtual string AgentType { get; set; }

        public string PrimaryColor { get; set; }

        public string SecondaryColor { get; set; }

        public string CreatedBy { get; set; }

        public string Target { get; set; }

        public string Image { get; set; }

        public virtual AgentTrigger AgentTrigger { get; set; }

        public virtual ICollection<Category> Categories { get; set; }

        public virtual ICollection<EventTrack> EventTracks { get; set; }

        public virtual ICollection<AgentRunner> AgentRunners { get; set; }
    }
}
