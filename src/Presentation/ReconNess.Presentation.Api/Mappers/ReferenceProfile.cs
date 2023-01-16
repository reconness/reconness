using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Mappers;

public class ReferenceProfile : Profile
{
    public ReferenceProfile()
    {
        CreateMap<ReferenceDto, Reference>().ReverseMap();
    }
}
