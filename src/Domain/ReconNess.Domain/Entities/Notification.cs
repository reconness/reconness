using System;

namespace ReconNess.Domain.Entities;

public class Notification : BaseEntity, IEntity
{
    public Guid Id { get; set; }

    public string Url { get; set; }

    public string Method { get; set; }

    public string Payload { get; set; }

    public string RootDomainPayload { get; set; }

    public string SubdomainPayload { get; set; }

    public string IpAddressPayload { get; set; }

    public string IsAlivePayload { get; set; }

    public string HasHttpOpenPayload { get; set; }

    public string TakeoverPayload { get; set; }

    public string DirectoryPayload { get; set; }

    public string ServicePayload { get; set; }

    public string NotePayload { get; set; }

    public string TechnologyPayload { get; set; }

    public string ScreenshotPayload { get; set; }
}
