using NLog;
using ReconNess.Core;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="INotesService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public NotesService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <inheritdoc/>
        public async Task SaveRootdomainNotesAsync(RootDomain rootDomain, string notesContent, CancellationToken cancellationToken = default)
        {
            var notes = rootDomain.Notes;
            if (notes == null)
            {
                notes = new Note
                {
                    Notes = notesContent,
                    RootDomain = rootDomain
                };

                await this.AddAsync(notes, cancellationToken);
            }
            else
            {
                notes.Notes = notesContent;

                await this.UpdateAsync(notes, cancellationToken);
            }
        }

        /// <inheritdoc/>
        public async Task SaveSubdomainNotesAsync(Subdomain subdomain, string notesContent, CancellationToken cancellationToken = default)
        {
            var notes = subdomain.Notes;
            if (notes == null)
            {
                notes = new Note
                {
                    Notes = notesContent,
                    Subdomain = subdomain
                };

                await this.AddAsync(notes, cancellationToken);
            }
            else
            {
                notes.Notes = notesContent;

                await this.UpdateAsync(notes, cancellationToken);
            }
        }
    }
}
