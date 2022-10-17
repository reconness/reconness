
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ReconNess
{
    /// <summary>
    /// The helper class
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ZipSerializedObject<T>(T obj)
        {
            var result = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy",
                ContractResolver = new SkipEmptyContractResolver()
            });

            result = Regex.Replace(result, @"\""Id\"":\""([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})\"",?", "");
            result = Regex.Replace(result, @"\""CreatedAt\"":\""([0-9]{4})\"",?", "");

            return Encoding.UTF8.GetBytes(result);
        }

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

    /// <summary>
    /// 
    /// </summary>
    public class SkipEmptyContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member,
                MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            bool isDefaultValueIgnored =
                ((property.DefaultValueHandling ?? DefaultValueHandling.Ignore)
                    & DefaultValueHandling.Ignore) != 0;
            if (isDefaultValueIgnored
                    && !typeof(string).IsAssignableFrom(property.PropertyType)
                    && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                Predicate<object> newShouldSerialize = obj =>
                {
                    var collection = property.ValueProvider.GetValue(obj) as ICollection;
                    return collection == null || collection.Count != 0;
                };
                Predicate<object> oldShouldSerialize = property.ShouldSerialize;
                property.ShouldSerialize = oldShouldSerialize != null
                    ? o => oldShouldSerialize(o) && newShouldSerialize(o)
                    : newShouldSerialize;
            }

            return property;
        }
    }
}
