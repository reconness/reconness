using NLog;
using ReconNess.Application.DataAccess;
using ReconNess.Application.Providers;
using ReconNess.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Services;

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
    public async Task<Note> AddTargetCommentAsync(Target target, string comment, CancellationToken cancellationToken = default)
    {
        Note note = new Note
        {
            Comment = comment,
            CreatedBy = authProvider.UserName(),
            Target = target
        };

        return await AddAsync(note, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Note> AddRootdomainCommentAsync(RootDomain rootDomain, string comment, CancellationToken cancellationToken = default)
    {
        Note note = new Note
        {
            Comment = comment,
            CreatedBy = authProvider.UserName(),
            RootDomain = rootDomain
        };

        return await AddAsync(note, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Note> AddSubdomainCommentAsync(Subdomain subdomain, string comment, CancellationToken cancellationToken = default)
    {
        Note note = new Note
        {
            Comment = comment,
            CreatedBy = authProvider.UserName(),
            Subdomain = subdomain
        };

        return await AddAsync(note, cancellationToken);
    }
}
