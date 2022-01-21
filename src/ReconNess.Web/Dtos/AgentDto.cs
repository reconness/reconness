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

        public string Script { get; set; }

        public DateTime LastRun { get; set; }

        public List<string> Categories { get; set; }

        public string AgentType { get; set; }

        public string PrimaryColor { get; set; }

        public string SecondaryColor { get; set; }

        public string CreatedBy { get; set; }

        public bool TriggerSkipIfRunBefore { get; set; }

        public bool TriggerTargetHasBounty { get; set; }

        public string TriggerTargetIncExcName { get; set; }

        public string TriggerTargetName { get; set; }

        public bool TriggerRootdomainHasBounty { get; set; }

        public string TriggerRootdomainIncExcName { get; set; }

        public string TriggerRootdomainName { get; set; }

        public bool TriggerSubdomainHasBounty { get; set; }

        public bool TriggerSubdomainIsAlive { get; set; }

        public bool TriggerSubdomainIsMainPortal { get; set; }

        public bool TriggerSubdomainHasHttpOrHttpsOpen { get; set; }

        public string TriggerSubdomainIncExcName { get; set; }

        public string TriggerSubdomainName { get; set; }

        public string TriggerSubdomainIncExcServicePort { get; set; }

        public string TriggerSubdomainServicePort { get; set; }

        public string TriggerSubdomainIncExcIP { get; set; }

        public string TriggerSubdomainIP { get; set; }

        public string TriggerSubdomainIncExcTechnology { get; set; }

        public string TriggerSubdomainTechnology { get; set; }

        public string TriggerSubdomainIncExcLabel { get; set; }

        public string TriggerSubdomainLabel { get; set; }

        public string ConfigurationFileName { get; set; }

        public string ConfigurationContent { get; set; } = "";

        public string ConfigurationPath { get; set; }
    }
}
