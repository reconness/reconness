using NLog;
using ReconNess.Core;
using ReconNess.Core.Providers;
using ReconNess.Core.Services;
using ReconNess.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="INotesService"/>
    /// </summary>
    public class NotesService : Service<Note>, IService<Note>, INotesService
    {
        protected static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IAuthProvider authProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="INotesService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="authProvider"><see cref="IAuthProvider"/></param>
        public NotesService(IUnitOfWork unitOfWork, IAuthProvider authProvider)
            : base(unitOfWork)
        {
            this.authProvider = authProvider;
        }

        /// <inheritdoc/>
        public async Task AddTargetCommentAsync(Target target, string comment, CancellationToken cancellationToken = default)
        {
            target.Notes.Add(new Note
            {
                Comment = comment,
                CreatedBy = authProvider.UserName(),
                Target = target
            });

            this.UnitOfWork.Repository<Target>().Update(target, cancellationToken);
            await this.UnitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task AddRootdomainCommentAsync(RootDomain rootDomain, string comment, CancellationToken cancellationToken = default)
        {
            rootDomain.Notes.Add(new Note
            {
                Comment = comment,
                CreatedBy = authProvider.UserName(),
                RootDomain = rootDomain
            });

            this.UnitOfWork.Repository<RootDomain>().Update(rootDomain, cancellationToken);
            await this.UnitOfWork.CommitAsync();
        }

        /// <inheritdoc/>
        public async Task AddSubdomainCommentAsync(Subdomain subdomain, string comment, CancellationToken cancellationToken = default)
        {
            subdomain.Notes.Add(new Note
            {
                Comment = comment,
                CreatedBy = authProvider.UserName(),
                Subdomain = subdomain
            });

            this.UnitOfWork.Repository<Subdomain>().Update(subdomain, cancellationToken);
            await this.UnitOfWork.CommitAsync();
        }        
    }
}
