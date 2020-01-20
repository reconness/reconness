using System;

namespace ReconNess.Entities
{
    public class ServiceHttpDirectory : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public string Directory { get; set; }

        public string StatusCode { get; set; }

        /// <summary>
        /// GET, POST, PATCH, DELETE
        /// </summary>
        public string Method { get; set; }

        public virtual ServiceHttp ServiceHttp { get; set; }
    }
}
