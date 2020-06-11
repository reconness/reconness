using System;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos
{
    public class AgentDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public bool IsBySubdomain { get; set; }

        public bool OnlyIfIsAlive { get; set; }

        public bool OnlyIfHasHttpOpen { get; set; }

        public bool SkipIfRanBefore { get; set; }

        public bool NotifyNewFound { get; set; }

        public string NotificationPayload { get; set; }

        public bool NotifyIfAgentDone { get; set; }

        public List<string> Categories { get; set; }

        public string Command { get; set; }

        public string Arguments { get; set; }

        public string Script { get; set; }

        public DateTime LastRun { get; set; }

        public bool IsRunning { get; set; }
    }
}
