using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ReverseMap();
    }
}
