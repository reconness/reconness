﻿using AutoMapper;
using ReconNess.Domain.Entities;
using ReconNess.Presentation.Api.Dtos;

namespace ReconNess.Presentation.Api.Mappers;

public class DirectoryProfile : Profile
{
    public DirectoryProfile()
    {
        CreateMap<Directory, DirectoryDto>()
            .ReverseMap();
    }
}