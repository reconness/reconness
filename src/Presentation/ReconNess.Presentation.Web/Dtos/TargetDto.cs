﻿using System;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos;

public class TargetDto
{
    public Guid? Id { get; set; }

    public string Name { get; set; }

    public string AgentsRanBefore { get; set; }

    public string InScope { get; set; }

    public string OutOfScope { get; set; }

    public string BugBountyProgramUrl { get; set; }

    public bool IsPrivate { get; set; }

    public string PrimaryColor { get; set; }

    public string SecondaryColor { get; set; }

    public List<RootDomainDto> RootDomains { get; set; }
}
