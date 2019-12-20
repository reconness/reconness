using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReconNess.Entities;
using ReconNess.Services;

namespace ReconNess.UnitTests
{
    [TestClass]
    public class AgentTests
    {
        [TestMethod]
        public void TestFierceOneParse()
        {
            var agent = new Agent
            {
                Name = "Fierce",
                Script = @"
                    var match = System.Text.RegularExpressions.Regex.Match(lineInput, @"".*?'(.*?)':\s'(.*?opera.*?)'"");
                    if (match.Success && match.Groups.Count == 3)
                    {
                        var subdomain = match.Groups[2].Value.EndsWith('.') ? match.Groups[2].Value.Substring(0, match.Groups[2].Value.Length - 1) : match.Groups[2].Value;
                        return new ReconNess.Core.Models.ScriptOutput { Ip = match.Groups[1].Value, Subdomain = subdomain };
                    }

                    return new ReconNess.Core.Models.ScriptOutput();"
            };

            var scriptEngineService = new ScriptEngineService();
            scriptEngineService.InintializeAgent(agent);

            var result = scriptEngineService.ParseInputAsync("{'107.167.110.206': 'cs-prod-vip-hopper.opera-mini.net.',", 0).Result;

            Assert.IsTrue(result.Subdomain == "cs-prod-vip-hopper.opera-mini.net");
            Assert.IsTrue(result.Ip == "107.167.110.206");
        }

        [TestMethod]
        public void TestFierceTwoParse()
        {
            var agent = new Agent
            {
                Name = "Fierce",
                Script = @"
                    var match = System.Text.RegularExpressions.Regex.Match(lineInput, @""^.*?:\s(.*?opera.*?)\s\((.*?)\)"");
                    if (match.Success && match.Groups.Count == 3)
                    {
                        var subdomain = match.Groups[1].Value.EndsWith('.') ? match.Groups[1].Value.Substring(0, match.Groups[1].Value.Length - 1) : match.Groups[1].Value;
                        return new ReconNess.Core.Models.ScriptOutput { Ip = match.Groups[2].Value, Subdomain = subdomain };
                    }

                    return new ReconNess.Core.Models.ScriptOutput();"
            };

            var scriptEngineService = new ScriptEngineService();
            scriptEngineService.InintializeAgent(agent);

            var result = scriptEngineService.ParseInputAsync("Found: pl.opera.com. (3.15.119.208)", 0).Result;

            Assert.IsTrue(result.Subdomain == "pl.opera.com");
            Assert.IsTrue(result.Ip == "3.15.119.208");
        }

        [TestMethod]
        public void TestFierceThreeParse()
        {
            var agent = new Agent
            {
                Name = "Fierce",
                Script = @"
                    var match = System.Text.RegularExpressions.Regex.Match(lineInput, @""^.*?:\s(.*?opera.*?)\s\((.*?)\)"");
                    if (match.Success && match.Groups.Count == 3)
                    {
                        var subdomain = match.Groups[1].Value.EndsWith('.') ? match.Groups[1].Value.Substring(0, match.Groups[1].Value.Length - 1) : match.Groups[1].Value;
                        return new ReconNess.Core.Models.ScriptOutput { Ip = match.Groups[2].Value, Subdomain = subdomain };
                    }

                    return new ReconNess.Core.Models.ScriptOutput();"
            };

            var scriptEngineService = new ScriptEngineService();
            scriptEngineService.InintializeAgent(agent);

            var result = scriptEngineService.ParseInputAsync("SOA: nic1.opera.com. (185.26.183.160)", 0).Result;

            Assert.IsTrue(result.Subdomain == "nic1.opera.com");
            Assert.IsTrue(result.Ip == "185.26.183.160");
        }

        [TestMethod]
        public void TestGoBusterOneParse()
        {
            var match = System.Text.RegularExpressions.Regex.Match("Found: acme5.opera.com", @"^Found:\s(.*opera.*)");
            if (match.Success)
            {
                var group = match.Groups;
                var ips = group[0].Value.Length;
            }

            var agent = new Agent
            {
                Name = "GoBuster",
                Script = @"
                    if (lineInputCount < 13)
                    {
	                    return new ReconNess.Core.Models.ScriptOutput();
                    }

                    var match = System.Text.RegularExpressions.Regex.Match(lineInput, @""^Found:\s(.*opera.*)"");
                    if (match.Success && match.Groups.Count == 2)
                    {
                        return new ReconNess.Core.Models.ScriptOutput { Subdomain = match.Groups[1].Value };
                    }

                    return new ReconNess.Core.Models.ScriptOutput(); "
            };

            var scriptEngineService = new ScriptEngineService();
            scriptEngineService.InintializeAgent(agent);

            var result = scriptEngineService.ParseInputAsync("Found: acme5.opera.com", 20).Result;

            Assert.IsTrue(result.Subdomain == "acme5.opera.com");
        }

        [TestMethod]
        public void TestNmapParse()
        {
            var agent = new Agent
            {
                Name = "Nmap",
                Script = @"
                    var match = System.Text.RegularExpressions.Regex.Match(lineInput, @""(.*?)/tcp\s*open\s*(.*?)$"");
                    if (match.Success && match.Groups.Count == 3)
                    {
                        return new ReconNess.Core.Models.ScriptOutput { Service = match.Groups[2].Value, Port = int.Parse(match.Groups[1].Value) };
                    }

                    return new ReconNess.Core.Models.ScriptOutput();"
            };

            var scriptEngineService = new ScriptEngineService();
            scriptEngineService.InintializeAgent(agent);

            var result = scriptEngineService.ParseInputAsync("22/tcp  open  ssh", 0).Result;

            Assert.IsTrue(result.Service == "ssh");
            Assert.IsTrue(result.Port == 22);
        }

    }
}
