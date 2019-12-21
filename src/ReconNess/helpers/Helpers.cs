
using System.Linq;
using System.Net;

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
    }
}
