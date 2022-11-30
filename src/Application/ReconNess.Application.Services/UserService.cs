using ReconNess.Application.DataAccess;
using ReconNess.Domain.Entities;

namespace ReconNess.Application.Services;

/// <summary>
/// This class implement <see cref="IUserService"/>
/// </summary>
public class UserService : Service<User>, IService<User>, IUserService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ILabelService" /> class
    /// </summary>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
    public UserService(IUnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }
}
