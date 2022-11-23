using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ReverseMap();
    }
}
