using ReconNess.Application.DataAccess;
using ReconNess.Domain.Entities;

namespace ReconNess.Application.Services;

/// <summary>
/// This class implement <see cref="IAgentsSettingService"/>
/// </summary>
public class AgentsSettingService : Service<AgentsSetting>, IService<AgentsSetting>, IAgentsSettingService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IAgentsSettingService" /> class
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
    public AgentsSettingService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }
}
