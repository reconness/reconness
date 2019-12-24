using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ReconNess.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class ReconnetHub : Hub
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        public async Task SendMessage(string logs)
        {
            await Clients.All.SendAsync("AgentRunLogs", logs);
        }
    }
}