using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;
using ReconNess.Web.Mappers.Resolvers;

namespace ReconNess.Web.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                 .ForMember(
                    dest => dest.Role, src => src.MapFrom<UserProfileResolver>()
                );

            CreateMap<UserDto, User>();
        }
    }
}
