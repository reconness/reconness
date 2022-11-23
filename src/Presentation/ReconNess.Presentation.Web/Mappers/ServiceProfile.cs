using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<Service, ServiceDto>()
            .ReverseMap();
    }
}
