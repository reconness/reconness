using System;

namespace ReconNess.Web.Dtos
{
    public class ServiceHttpDirectoryDto
    {
        public Guid? Id { get; set; }

        public string Directory { get; set; }

        public string StatusCode { get; set; }

        public string Size { get; set; }

        /// <summary>
        /// GET, POST, PATCH, DELETE
        /// </summary>
        public string Method { get; set; }
    }
}
