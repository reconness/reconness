using System;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos;

public class SubdomainDto
{
    public Guid? Id { get; set; }

    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Target { get; set; }

    public string RootDomain { get; set; }

    public string IpAddress { get; set; }

    public bool? Takeover { get; set; }

    public bool? HasHttpOpen { get; set; }

    public bool? IsMainPortal { get; set; }

    public bool? IsAlive { get; set; }

    public string Screenshot { get; set; }

    public string Technology { get; set; }

    public string AgentsRanBefore { get; set; }

    public List<DirectoryDto> Directories { get; set; }

    public List<ServiceDto> Services { get; set; }

    public List<LabelDto> Labels { get; set; }
}
