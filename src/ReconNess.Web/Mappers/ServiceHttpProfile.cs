using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers
{
    public class ServiceHttpProfile : Profile
    {
        public ServiceHttpProfile()
        {
            CreateMap<ServiceHttp, ServiceHttpDto>()
                .ReverseMap();
        }
    }
}
