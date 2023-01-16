using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Mappers;

public class RootDomainProfile : Profile
{
    public RootDomainProfile()
    {
        CreateMap<RootDomain, RootDomainDto>().ReverseMap();
    }
}
