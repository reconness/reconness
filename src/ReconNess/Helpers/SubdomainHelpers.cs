using System;
using System.IO;
using System.Reflection;

namespace ReconNess.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public static class SubdomainHelpers
    {
        /// <summary>
        /// Obtain the base64 screenshot if exist
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="rootdomain">The rootdomain</param>
        /// <param name="subdomain">The subdomain</param>
        /// <returns>The base64 screenshot if exist</returns>
        public static string GetBase64Image(string target, string rootdomain, string subdomain)
        {
            target = RemoveInvalidFileNameChars(target);
            rootdomain = RemoveInvalidFileNameChars(rootdomain);
            subdomain = RemoveInvalidFileNameChars(subdomain);

            var screenshotPath = GetScreenshotPath();
            var path = Path.Combine(screenshotPath, target, rootdomain, $"{subdomain}.png");
            if (!path.StartsWith(screenshotPath) || !File.Exists(path))
            {
                return null;
            }

            byte[] imageArray = File.ReadAllBytes(path);
            return Convert.ToBase64String(imageArray);
        }

        /// <summary>
        /// Obtain the screenshot folder path
        /// </summary>
        /// <returns>The screenshot folder path</returns>
        private static string GetScreenshotPath()
        {
            var bin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(bin, "Content", "screenshots");

            return path;
        }

        /// <summary>
        /// Obtain the string with removed invalid file name chars
        /// </summary>
        /// <param name="input">The string to remove invalid file name chars</param>
        /// <returns>The string with removed invalid file name chars</returns>
        private static string RemoveInvalidFileNameChars(string input)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                input = input.Replace(c.ToString(), "");
            }

            return input;
        }
    }
}
