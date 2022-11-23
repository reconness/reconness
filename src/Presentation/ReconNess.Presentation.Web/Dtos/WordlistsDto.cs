using ReconNess.Application.Models;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos;

public class WordlistsDto
{
    public List<Wordlist> SubdomainsEnum { get; set; } = new List<Wordlist>();

    public List<Wordlist> DirectoriesEnum { get; set; } = new List<Wordlist>();

    public List<Wordlist> DNSResolvers { get; set; } = new List<Wordlist>();
}
