using ReconNess.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ReconNess.Application.Services;

/// <summary>
/// The interface INotesService
/// </summary>
public interface INotesService : IService<Note>
{
     /// <summary>
     /// Save rootdomain comment
     /// </summary>
     /// <param name="target">The target</param>
     /// <param name="comment">The notes</param>
     /// <param name="cancellationToken">Notification that operations should be canceled</param>
     /// <returns></returns>
    Task<Note> AddTargetCommentAsync(Target target, string comment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save rootdomain comment
    /// </summary>
    /// <param name="rootDomain">The rootdomain</param>
    /// <param name="comment">The notes</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns></returns>
    Task<Note> AddRootdomainCommentAsync(RootDomain rootDomain, string comment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save subdomains notes
    /// </summary>
    /// <param name="subdomain">The subdomain</param>
    /// <param name="comment">The notes</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns></returns>
    Task<Note> AddSubdomainCommentAsync(Subdomain subdomain, string comment, CancellationToken cancellationToken = default);
}
