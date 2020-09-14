using System;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos
{
    public class AgentDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Repository { get; set; }

        public string Command { get; set; }

        public string Arguments { get; set; }

        public string Script { get; set; }

        public DateTime LastRun { get; set; }

        public List<string> Categories { get; set; }

        public bool IsRunning { get; set; }
    }
}
