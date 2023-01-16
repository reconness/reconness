using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Mappers;

public class TargetProfile : Profile
{
    public TargetProfile()
    {
        CreateMap<Target, TargetDto>()
            .ReverseMap();
    }
}
