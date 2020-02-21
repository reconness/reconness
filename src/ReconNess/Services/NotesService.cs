using System.Threading;
using System.Threading.Tasks;
using ReconNess.Core;
using ReconNess.Core.Services;
using ReconNess.Entities;

namespace ReconNess.Services
{
    /// <summary>
    /// This class implement <see cref="INotesService"/>
    /// </summary>
    public class NotesService : Service<Note>, IService<Note>, INotesService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INotesService" /> class
        /// </summary>
        /// <param name="unitOfWork"><see cref="IUnitOfWork"/></param>
        public NotesService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <summary>
        /// <see cref="INotesService.SaveTargetNotesAsync(Target, string, CancellationToken)"/>
        /// </summary>
        public async Task SaveTargetNotesAsync(Target target, string notesContent, CancellationToken cancellationToken = default)
        {
            var notes = target.Notes;
            if (notes == null)
            {
                notes = new Note
                {
                    Notes = notesContent,
                    Target = target
                };

                await this.AddAsync(notes, cancellationToken);
            }
            else
            {
                notes.Notes = notesContent;

                await this.UpdateAsync(notes, cancellationToken);
            }
        }

        /// <summary>
        /// <see cref="INotesService.SaveSubdomainNotesAsync(Subdomain, string, CancellationToken)"/>
        /// </summary>
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
