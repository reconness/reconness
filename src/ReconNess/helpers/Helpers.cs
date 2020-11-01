
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ReconNess.Helpers
{
    /// <summary>
    /// The helper class
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// IP4 validation
        /// </summary>
        /// <param name="ipString">The Ip to validate</param>
        /// <returns>If is a valid IP</returns>
        public static bool ValidateIPv4(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString) || ipString.Count(c => c == '.') != 3)
            {
                return false;
            }

            return IPAddress.TryParse(ipString, out IPAddress address);
        }

        public static byte[] ZipSerializedObject<T>(T obj, string target, string rootdomain)
        {
            var result = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "yyyy",
                ContractResolver = new SkipEmptyContractResolver()
            });

            result = Regex.Replace(result, @"\""Id\"":\""([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})\"",", "");
            result = Regex.Replace(result, @"\""CreatedAt\"":\""([0-9]{4})\"",", "");
            result = Regex.Replace(result, string.Format(@",?\""Target\"":\""{0}\""", target), "");
            result = Regex.Replace(result, string.Format(@",?\""RootDomain\"":\""{0}\""", rootdomain), "");

            return Encoding.UTF8.GetBytes(result);
        }
    }

    public class SkipEmptyContractResolver : DefaultContractResolver
    {
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
