using System;

namespace ReconNess.Entities
{
    public class AgentCategory : BaseEntity, IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid AgentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Agent Agent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Category Category { get; set; }
    }
}
