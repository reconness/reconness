using ReconNess.Core.Models;
using ReconNess.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Core.Services
{
    /// <summary>
    /// The interface IAgentTypeService
    /// </summary>
    public interface IAgentTypeService : IService<Type>
    {
        /// <summary>
        /// Obtain the list of types from database
        /// </summary>
        /// <param name="myTypes">The list of my types</param>
        /// <param name="agentTypeModel">The new types</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of types</returns>
        Task<ICollection<AgentType>> GetTypesAsync(ICollection<AgentType> myTypes, AgentTypeModel agentTypeModel, CancellationToken cancellationToken = default);
    }
}
