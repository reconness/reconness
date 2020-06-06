using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ReconNess.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class ReconNessHub : Hub
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