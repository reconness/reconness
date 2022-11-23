using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers;

public class EventTrackProfile : Profile
{
    public EventTrackProfile()
    {
        CreateMap<EventTrack, EventTrackDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
