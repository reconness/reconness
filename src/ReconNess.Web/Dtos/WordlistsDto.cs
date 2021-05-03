using ReconNess.Core.Models;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos
{
    public class WordlistsDto
    {
        public List<Wordlist> SubdomainsEnum { get; set; }

        public List<Wordlist> DirectoriesEnum { get; set; }

        public List<Wordlist> DNSResolvers { get; set; }
    }
}
