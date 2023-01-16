using System;

namespace ReconNess.Presentation.Api.Dtos;

public class ServiceDto
{
    public Guid? Id { get; set; }

    public string Name { get; set; }

    public int Port { get; set; }
}
