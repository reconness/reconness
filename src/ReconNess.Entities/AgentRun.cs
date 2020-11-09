using ReconNess.Entities.Enum;
using System;

namespace ReconNess.Entities
{
    public class AgentRun : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Channel { get; set; }

        public string Description { get; set; }

        public AgentRunStage Stage { get; set; }

        public string TerminalOutput { get; set; }

        public string Logs { get; set; }

        public virtual Agent Agent { get; set; }
    }
}
