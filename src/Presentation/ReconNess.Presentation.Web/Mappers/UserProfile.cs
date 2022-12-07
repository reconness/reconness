using AutoMapper;
using ReconNess.Infrastructure.Identity.Entities;
using ReconNess.Presentation.Api.Dtos;
using ReconNess.Presentation.Api.Mappers.Resolvers;

namespace ReconNess.Presentation.Api.Mappers;

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
