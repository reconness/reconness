using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="ITargetService"/>
    /// </summary>
    public class TargetService : Service<Target>, IService<Target>, ITargetService
    {
        private readonly IRootDomainService rootDomainService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ILabelService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public TargetService(IUnitOfWork unitOfWork, IRootDomainService rootDomainService)
            : base(unitOfWork)
        {
            this.rootDomainService = rootDomainService;
        }

        public async Task DeleteTargetAsync(Target target, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                this.UnitOfWork.BeginTransaction(cancellationToken);

                this.rootDomainService.DeleteRootDomains(target.RootDomains, cancellationToken);
                this.UnitOfWork.Repository<Target>().Delete(target, cancellationToken);

                await this.UnitOfWork.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                this.UnitOfWork.Rollback(cancellationToken);
                throw ex;
            }
        }
    }
}
