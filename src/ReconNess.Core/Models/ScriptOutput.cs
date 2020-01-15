namespace ReconNess.Core.Models
{
    public class ScriptOutput
    {
        public string Subdomain { get; set; }

        public string Ip { get; set; }

        public bool? Takeover { get; set; }

        public bool? IsAlive { get; set; }

        public bool? HasHttpOpen { get; set; }

        public bool? HasScreenshot { get; set; }

        public int? Port { get; set; }

        public string Service { get; set; }
    }
}
