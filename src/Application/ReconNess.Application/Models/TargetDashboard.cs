using System;
using System.Collections.Generic;

namespace ReconNess.Application.Models;

public  class TargetDashboard
{
    public IEnumerable<SubdomainByPort> SubdomainByPort { get; set; }

    public IEnumerable<SubdomainByDirectories> SubdomainByDirectories { get; set; }

    public IEnumerable<DashboardEventTrackInteraction> Interactions { get; set; }

    public IEnumerable<DashboardEventTrack> EventTracks { get; set; }
}

public class SubdomainByPort
{
    public int Port { get; set; }

    public int Count { get; set; }
}

public class SubdomainByDirectories
{
    public string Subdomain { get; set; }

    public int Count { get; set; }
}

public class DashboardEventTrackInteraction
{
    public DayOfWeek Day { get; set; }
    public int Count { get; set; }
}

public class DashboardEventTrack
{
    public DateTime Date { get; set; }
    public string CreatedBy { get; set; }
    public string Data { get; set; }
}
