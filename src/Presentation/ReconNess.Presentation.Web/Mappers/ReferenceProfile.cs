using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers
{
    public class ReferenceProfile : Profile
    {
        public ReferenceProfile()
        {
            CreateMap<ReferenceDto, Reference>().ReverseMap();
        }
    }
}
