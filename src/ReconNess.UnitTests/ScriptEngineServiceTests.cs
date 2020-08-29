using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReconNess.Services;

namespace ReconNess.UnitTests
{
    [TestClass]
    public class ScriptEngineServiceTests
    {
        [TestMethod]
        public void TestFierceOneParseInputAsyncMethod()
        {
            // Arrange

            var script = @"
                    var match = System.Text.RegularExpressions.Regex.Match(lineInput, @""^.*?:\s(.*?opera.*?)\s\((.*?)\)"");
                    if (match.Success && match.Groups.Count == 3)
                    {
                        var subdomain = match.Groups[1].Value.EndsWith('.') ? match.Groups[1].Value.Substring(0, match.Groups[1].Value.Length - 1) : match.Groups[1].Value;
                        return new ReconNess.Core.Models.ScriptOutput { Ip = match.Groups[2].Value, Subdomain = subdomain };
                    }

                    return new ReconNess.Core.Models.ScriptOutput();";


            var scriptEngineService = new ScriptEngineService();

            // Act
            var result = scriptEngineService.TerminalOutputParseAsync(script, "Found: pl.opera.com. (3.15.119.208)", 0).Result;

            // Assert
            Assert.IsTrue(result.Subdomain == "pl.opera.com");
            Assert.IsTrue(result.Ip == "3.15.119.208");
        }

        [TestMethod]
        public void TestFierceTwoParseInputAsyncMethod()
        {
            // Arrange
            var script = @"
                    var match = System.Text.RegularExpressions.Regex.Match(lineInput, @""^.*?:\s(.*?opera.*?)\s\((.*?)\)"");
                    if (match.Success && match.Groups.Count == 3)
                    {
                        var subdomain = match.Groups[1].Value.EndsWith('.') ? match.Groups[1].Value.Substring(0, match.Groups[1].Value.Length - 1) : match.Groups[1].Value;
                        return new ReconNess.Core.Models.ScriptOutput { Ip = match.Groups[2].Value, Subdomain = subdomain };
                    }

                    return new ReconNess.Core.Models.ScriptOutput();";


            var scriptEngineService = new ScriptEngineService();

            // Act
            var result = scriptEngineService.TerminalOutputParseAsync(script, "SOA: nic1.opera.com. (185.26.183.160)", 0).Result;

            // Assert
            Assert.IsTrue(result.Subdomain == "nic1.opera.com");
            Assert.IsTrue(result.Ip == "185.26.183.160");
        }

        [TestMethod]
        public void TestGoBusterOneParseInputAsyncMethod()
        {
            // Arrange
            var match = System.Text.RegularExpressions.Regex.Match("Found: acme5.opera.com", @"^Found:\s(.*opera.*)");
            if (match.Success)
            {
                var group = match.Groups;
                var ips = group[0].Value.Length;
            }

            var script = @"
                    if (lineInputCount < 13)
                    {
	                    return new ReconNess.Core.Models.ScriptOutput();
                    }

                    var match = System.Text.RegularExpressions.Regex.Match(lineInput, @""^Found:\s(.*opera.*)"");
                    if (match.Success && match.Groups.Count == 2)
                    {
                        return new ReconNess.Core.Models.ScriptOutput { Subdomain = match.Groups[1].Value };
                    }

                    return new ReconNess.Core.Models.ScriptOutput(); ";

            var scriptEngineService = new ScriptEngineService();

            // Act
            var result = scriptEngineService.TerminalOutputParseAsync(script, "Found: acme5.opera.com", 20).Result;

            // Assert
            Assert.IsTrue(result.Subdomain == "acme5.opera.com");
        }

        [TestMethod]
        public void TestNmapParseInputAsyncMethod()
        {
            // Arrange
            var script = @"
                    var match = System.Text.RegularExpressions.Regex.Match(lineInput, @""(.*?)/tcp\s*open\s*(.*?)$"");
                    if (match.Success && match.Groups.Count == 3)
                    {
                        return new ReconNess.Core.Models.ScriptOutput { Service = match.Groups[2].Value, Port = int.Parse(match.Groups[1].Value) };
                    }

                    return new ReconNess.Core.Models.ScriptOutput();";

            var scriptEngineService = new ScriptEngineService();

            // Act
            var result = scriptEngineService.TerminalOutputParseAsync(script, "22/tcp  open  ssh", 0).Result;

            // Assert
            Assert.IsTrue(result.Service == "ssh");
            Assert.IsTrue(result.Port == 22);
        }
    }
}
