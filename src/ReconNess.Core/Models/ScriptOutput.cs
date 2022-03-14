﻿namespace ReconNess.Core.Models
{
    public class ScriptOutput
    {
        public string RootDomain { get; set; }

        public string Subdomain { get; set; }

        public string Ip { get; set; }

        public bool? Takeover { get; set; }

        public bool? IsAlive { get; set; }

        public bool? HasHttpOpen { get; set; }

        public int? Port { get; set; }

        public string Service { get; set; }

        public string HttpDirectory { get; set; }

        public string HttpDirectoryMethod { get; set; }

        public string HttpDirectoryStatusCode { get; set; }

        public string HttpDirectorySize { get; set; }

        public string Label { get; set; }

        public string Note { get; set; }

        public string Technology { get; set; }
    }
}
