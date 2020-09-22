using ReconNess.Core;
using ReconNess.Core.Models;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="IAgentTypeService"/>
    /// </summary>
    public class AgentTypeService : Service<Type>, IService<Type>, IAgentTypeService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IAgentTypeService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public AgentTypeService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <summary>
        /// <see cref="IAgentTypeService.GetTypesAsync(List{AgentType}, AgentTypeModel, CancellationToken)"/>
        /// </summary>
        public async Task<ICollection<AgentType>> GetTypesAsync(ICollection<AgentType> myTypes, AgentTypeModel agentTypeModel, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var newTypes = new List<AgentType>();

            var types = await this.GetAllAsync(cancellationToken);

            if (agentTypeModel.IsByTarget)
            {
                newTypes.Add(this.GetAgentType(myTypes, types, AgentTypes.TARGET));
            }

            if (agentTypeModel.IsByRootDomain)
            {
                newTypes.Add(this.GetAgentType(myTypes, types, AgentTypes.ROOTDOMAIN));
            }

            if (agentTypeModel.IsBySubdomain)
            {
                newTypes.Add(this.GetAgentType(myTypes, types, AgentTypes.SUBDOMAIN));
            }

            if (agentTypeModel.IsByDirectory)
            {
                newTypes.Add(this.GetAgentType(myTypes, types, AgentTypes.DIRECTORY));
            }

            if (agentTypeModel.IsByResource)
            {
                newTypes.Add(this.GetAgentType(myTypes, types, AgentTypes.RESOURCE));
            }

            return newTypes;
        }

        /// <summary>
        /// Obtain the agent type from the current agent types or the database
        /// </summary>
        /// <param name="myTypes">My current agent types</param>
        /// <param name="types">all the types from the database</param>
        /// <param name="type">The Type that we need to obtain</param>
        /// <returns>The agent type from the current agent types or the database</returns>
        private AgentType GetAgentType(ICollection<AgentType> myTypes, List<Type> types, string type)
        {
            var agentType = myTypes.FirstOrDefault(t => t.Type.Name == type);
            if (agentType != null)
            {
                return agentType;
            }
            
            return new AgentType
            {
                TypeId = types.First(t => t.Name == type).Id
            };            
        }
    }
}
