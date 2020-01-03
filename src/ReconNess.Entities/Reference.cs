using System;

namespace ReconNess.Entities
{
    public class Reference : BaseEntity, IEntity
    {
        public Guid Id { get; set; }

        public String Url { get; set; }

        public string Categories { get; set; }
    }
}
