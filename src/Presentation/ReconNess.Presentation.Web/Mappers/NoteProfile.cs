using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers;

public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<Note, NoteDto>()
            .ReverseMap();
    }
}
