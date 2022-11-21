using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers
{
    public class DirectoryProfile : Profile
    {
        public DirectoryProfile()
        {
            CreateMap<Directory, DirectoryDto>()
                .ReverseMap();
        }
    }
}
