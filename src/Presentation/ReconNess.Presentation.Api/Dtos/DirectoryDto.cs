using System;

namespace ReconNess.Presentation.Api.Dtos;

public class DirectoryDto
{
    public Guid? Id { get; set; }

    public string Uri { get; set; }

    public string StatusCode { get; set; }

    public string Size { get; set; }

    public string Method { get; set; }
}
