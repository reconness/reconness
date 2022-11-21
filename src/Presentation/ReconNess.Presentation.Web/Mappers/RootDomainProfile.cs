using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers
{
    public class RootDomainProfile : Profile
    {
        public RootDomainProfile()
        {
            CreateMap<RootDomain, RootDomainDto>().ReverseMap();
        }
    }
}
