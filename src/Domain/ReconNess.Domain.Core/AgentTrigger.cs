using System;

namespace ReconNess.Entities
{
    public class AgentTrigger : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public bool? SkipIfRunBefore { get; set; }

        public bool? TargetHasBounty { get; set; }

        public string TargetIncExcName { get; set; }

        public string TargetName { get; set; }

        public bool? RootdomainHasBounty { get; set; }

        public string RootdomainIncExcName { get; set; }

        public string RootdomainName { get; set; }

        public bool? SubdomainHasBounty { get; set; }

        public bool? SubdomainIsAlive { get; set; }

        public bool? SubdomainIsMainPortal { get; set; }

        public bool? SubdomainHasHttpOrHttpsOpen { get; set; }

        public string SubdomainIncExcName { get; set; }

        public string SubdomainName { get; set; }

        public string SubdomainIncExcServicePort { get; set; }

        public string SubdomainServicePort { get; set; }

        public string SubdomainIncExcIP { get; set; }

        public string SubdomainIP { get; set; }

        public string SubdomainIncExcTechnology { get; set; }

        public string SubdomainTechnology { get; set; }

        public string SubdomainIncExcLabel { get; set; }

        public string SubdomainLabel { get; set; }

        public Guid AgentId { get; set; }
        public virtual Agent Agent { get; set; }
    }
}
