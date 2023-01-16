using System;

namespace ReconNess.Presentation.Api.Dtos;

public class NoteDto
{
    public Guid Id { get; set; }

    public string CreatedBy { get; set; }

    public string Comment { get; set; }
}
