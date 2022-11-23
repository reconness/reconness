using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers;

public class LabelProfile : Profile
{
    public LabelProfile()
    {
        CreateMap<Label, LabelDto>()
            .ReverseMap();
    }
}
