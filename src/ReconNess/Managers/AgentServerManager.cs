using Microsoft.Extensions.DependencyInjection;
using ReconNess.Core;
using ReconNess.Core.Managers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Managers
{
    /// <summary>
    /// This class implement <see cref="IAgentServerManager"/>, this implemntation does not check the pub/sub service directly, to know what server is unbusy, is a decoupling
    /// of what is happending in the pub/sub service, that can be an issue, but at the same time is not big deal to be decoupling of the pub/sub service
    /// and emulate what could be happening base on the traffic
    /// </summary>
    public class AgentServerManager : IAgentServerManager
    {
        private readonly IAgentServerSetting agentServerSetting;

        private AgentsSetting agentsSetting;

        private ConcurrentDictionary<int, AgentServer> servers = new ConcurrentDictionary<int, AgentServer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IAgentServerManager" /> class
        /// </summary>
        /// <param name="agentServerSetting"><see cref="IAgentServerSetting"/></param>
        public AgentServerManager(IAgentServerSetting agentServerSetting)
        {
            this.agentServerSetting = agentServerSetting;
        }

        /// <inheritdoc/>
        public async Task<int> GetAvailableServerAsync(string channel, int refreshInMin = 60, CancellationToken cancellationToken = default)
        {
            await this.InitialiceAsync(cancellationToken);

            // try to remove the keys inside the servers with more than 1 hr created
            this.RefreshServers(refreshInMin);

            if (this.agentsSetting.Strategy == Entities.Enum.AgentRunnerStrategy.ROUND_ROBIN)
            {
                return this.GetServerChannelOrUnbusy(channel);
            }

            return this.GetServerUnbusy(channel);
        }

        /// <summary>
        /// If the Setting is null obtain it from the database, and initialice the servers track 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A Task</returns>
        private async Task InitialiceAsync(CancellationToken cancellationToken)
        {
            if (this.agentsSetting == null)
            {
                this.agentsSetting = await agentServerSetting.GetAgentSettingAsync(cancellationToken);
                for (int i = 1; i <= this.agentsSetting.AgentServerCount; i++)
                {
                    this.servers.TryAdd(i, new AgentServer());
                }
            }
        }

        /// <summary>
        /// Remove the keys inside the servers with more than 1 hr created
        /// </summary>
        private void RefreshServers(int refreshInMin)
        {
            for (int i = 1; i <= this.agentsSetting.AgentServerCount; i++)
            {
                this.servers[i].Refresh(refreshInMin);
            }
        }

        /// <summary>
        /// Obtain the server that is using this channel for roun robin or obtain the unbusy one 
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private int GetServerChannelOrUnbusy(string channel)
        {
            int unbusyServer = 1;
            int unbusyServerCount = int.MaxValue;
            for (int i = 1; i <= this.agentsSetting.AgentServerCount; i++)
            {
                if (this.servers[i].Any(channel))
                {
                    this.servers[i].Inc(channel);
                    return i;
                }

                if (unbusyServerCount > this.servers[i].Count)
                {
                    unbusyServer = i;
                    unbusyServerCount = this.servers[i].Count;
                }
            }

            this.servers[unbusyServer].Add(channel);

            return unbusyServer;
        }

        /// <summary>
        /// Obtain the server that is unbusy 
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private int GetServerUnbusy(string channel)
        {
            int unbusyServer = 1;
            int unbusyServerCount = int.MaxValue;
            for (int i = 1; i <= this.agentsSetting.AgentServerCount; i++)
            {
                if (unbusyServerCount > this.servers[i].Count)
                {
                    unbusyServer = i;
                    unbusyServerCount = this.servers[i].Count;
                }
            }

            if (this.servers[unbusyServer].Any(channel))
            {
                this.servers[unbusyServer].Inc(channel);
            }
            else
            {
                this.servers[unbusyServer].Add(channel);
            }

            return unbusyServer;
        }
    }
}
