﻿using ReconNess.Domain.Entities;

namespace ReconNess.Application.Models;

public class AgentRunnerInfo
{
    public Agent Agent { get; set; }
    public Target Target { get; set; }
    public RootDomain RootDomain { get; set; }
    public Subdomain Subdomain { get; set; }
    public string Command { get; set; }
    public bool ActivateNotification { get; set; }
}