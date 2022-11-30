using ReconNess.Application.DataAccess;
using ReconNess.Domain.Entities;

namespace ReconNess.Application.Services;

/// <summary>
/// This class implement <see cref="IRoleService"/>
/// </summary>
public class RoleService : Service<Role>, IService<Role>, IRoleService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IRoleService" /> class
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
    public RoleService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }
}
