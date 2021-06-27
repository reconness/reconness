using System;
using System.IO;
using System.Reflection;

namespace ReconNess.Helpers
{
    public static class SubdomainHelpers
    {
        public static string GetBase64Image(string target, string rootdomain, string subdomain)
        {
            var subdomainScreenshot = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content", "screenshots", target, rootdomain, $"{subdomain}.png");
            if (!File.Exists(subdomainScreenshot))
            {
                return null;
            }

            byte[] imageArray = File.ReadAllBytes(subdomainScreenshot);
            return Convert.ToBase64String(imageArray);
        }
    }
}
