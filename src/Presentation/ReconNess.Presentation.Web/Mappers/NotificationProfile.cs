using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationDto>()
                .ReverseMap();
        }
    }
}
