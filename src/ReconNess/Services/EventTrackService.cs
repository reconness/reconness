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
    /// This class implement <see cref="IEventTrackService"/>
    /// </summary>
    public class EventTrackService : Service<EventTrack>, IService<EventTrack>, IEventTrackService
    {
        private readonly IAuthProvider authProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="IEventTrackService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        /// <param name="authProvider"><see cref="IAuthProvider"/></param>
        public EventTrackService(IUnitOfWork unitOfWork,
            IAuthProvider authProvider)
            : base(unitOfWork)
        {
            this.authProvider = authProvider;
        }

        public override async Task<EventTrack> AddAsync(EventTrack entity, CancellationToken cancellationToken = default)
        {
            entity.Username = this.authProvider.UserName();

            return await base.AddAsync(entity, cancellationToken);
        }
    }
}
