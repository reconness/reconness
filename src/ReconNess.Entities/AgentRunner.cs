﻿using ReconNess.Entities.Enum;
using System;
using System.Collections.Generic;

namespace ReconNess.Entities
{
    public class AgentRunner : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Channel { get; set; }

        public AgentRunnerStage Stage { get; set; }

        public int Total { get; set; }

        public bool AllowSkip { get; set; }

        public string AgentRunnerType { get; set; }

        public bool ActivateNotification { get; set; }

        public virtual Agent Agent { get; set; }        

        public virtual ICollection<AgentRunnerCommand> Commands { get; set; }
    }
}
