using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ReconNess.Managers
{
    internal class AgentServer
    {
        private ConcurrentDictionary<string, AgentServerChannel> Channels = new ConcurrentDictionary<string, AgentServerChannel>();

        internal void Add(string channel)
        {
            this.Channels.TryAdd
            (
                channel,
                new AgentServerChannel
                {
                    Count = 1,
                    CreatedAt = DateTime.Now 
                }
            );
        }

        internal void Inc(string channel) => Channels[channel].Count++;

        internal bool Any(string channel) => Channels.ContainsKey(channel);

        internal int Count => Channels.Values.Sum(c => c.Count);

        internal void Refresh(int refreshTimeOnMin)
        {
            var channelsToRemove = new List<string>();
            foreach (var channel in Channels)
            {
                if (channel.Value.CreatedAt.AddMinutes(refreshTimeOnMin) < DateTime.Now)
                {
                    channelsToRemove.Add(channel.Key);
                }
            }

            foreach (var channelToRemove in channelsToRemove)
            {
                Channels.TryRemove(channelToRemove, out AgentServerChannel channel);
            }
        }
    }
}
