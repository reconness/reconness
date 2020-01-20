using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers
{
    public class ServiceHttpDirectoryProfile : Profile
    {
        public ServiceHttpDirectoryProfile()
        {
            CreateMap<ServiceHttpDirectory, ServiceHttpDirectoryDto>()
                .ReverseMap();
        }
    }
}
