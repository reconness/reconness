﻿using System;

namespace ReconNess.Entities
{
    public class AgentType : BaseEntity, IEntity
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
        public Guid TypeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Type Type { get; set; }
    }
}
