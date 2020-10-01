using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentTypeService"/>
    /// </summary>
    public class AgentTypeService : Service<AgentType>, IService<AgentType>, IAgentTypeService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IAgentTypeService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public AgentTypeService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
