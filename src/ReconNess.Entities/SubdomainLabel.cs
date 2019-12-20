using System;

namespace ReconNess.Entities
{
    public class SubdomainLabel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid SubdomainId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Subdomain Subdomain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid LabelId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Label Label { get; set; }
    }
}
