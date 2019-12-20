using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ReconNess.Web
{
    public class ReconnetHub : Hub
    {
        public async Task SendMessage(string logs)
        {
            await Clients.All.SendAsync("AgentRunLogs", logs);
        }
    }
}