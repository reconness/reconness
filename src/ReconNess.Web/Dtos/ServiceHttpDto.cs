using System;
using System.Collections.Generic;

namespace ReconNess.Web.Dtos
{
    public class ServiceHttpDto
    {
        public Guid? Id { get; set; }

        public string ScreenshotHttpPNGBase64 { get; set; }

        public string ScreenshotHttpsPNGBase64 { get; set; }

        public virtual ICollection<ServiceHttpDirectoryDto> Directories { get; set; }
    }
}
