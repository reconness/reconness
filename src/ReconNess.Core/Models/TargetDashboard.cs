using System;
using System.Collections.Generic;

namespace ReconNess.Core.Models
{
    public  class TargetDashboard
    {
        public IEnumerable<SubdomainByPort> SubdomainByPort { get; set; }

        public IEnumerable<SubdomainByDirectories> SubdomainByDirectories { get; set; }

        public IEnumerable<Interaction> Interactions { get; set; }

        public IEnumerable<DashboardLog> Logs { get; set; }
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

    public class Interaction
    {
        public DayOfWeek Day { get; set; }
        public int Count { get; set; }
    }

    public class DashboardLog
    {
        public DateTime Date { get; set; }
        public string Log { get; set; }
    }
}
