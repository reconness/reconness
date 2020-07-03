using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers.Resolvers
{
    internal class AgentNotificationResolver : IValueResolver<AgentDto, Agent, AgentNotification>
    {
        public AgentNotification Resolve(AgentDto source, Agent destination, AgentNotification member, ResolutionContext context)
        {
            return new AgentNotification
            {
                SubdomainPayload = source.SubdomainPayload,
                IpAddressPayload = source.IpAddressPayload,
                IsAlivePayload = source.IsAlivePayload,
                HasHttpOpenPayload = source.HasHttpOpenPayload,
                TakeoverPayload = source.TakeoverPayload,
                DirectoryPayload = source.DirectoryPayload,
                ServicePayload = source.ServicePayload,
                NotePayload = source.NotePayload
            };
        }
    }
}