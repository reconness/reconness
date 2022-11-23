using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers;

public class RootDomainProfile : Profile
{
    public RootDomainProfile()
    {
        CreateMap<RootDomain, RootDomainDto>().ReverseMap();
    }
}
