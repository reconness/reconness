using AutoMapper;
using ReconNess.Entities;
using ReconNess.Web.Dtos;

namespace ReconNess.Web.Mappers
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<Note, NoteDto>()
                .ReverseMap();
        }
    }
}
