using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Mappers;

public class LabelProfile : Profile
{
    public LabelProfile()
    {
        CreateMap<Label, LabelDto>()
            .ReverseMap();
    }
}
