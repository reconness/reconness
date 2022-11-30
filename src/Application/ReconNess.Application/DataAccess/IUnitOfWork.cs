namespace ReconNess.Application.DataAccess;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// This interface define a Unit of Work patter
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Obtain a generic repository
    /// </summary>
    /// <returns>A generic repository</returns>
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;

    /// <summary>
    /// Obtain a custom respository
    /// </summary>
    /// <typeparam name="TCustomRepo">The custom repository</typeparam>
    /// <typeparam name="TEntity">The entity that repository implement</typeparam>
    /// <returns>A custom respository</returns>
    TCustomRepo Repository<TCustomRepo, TEntity>() where TCustomRepo : IRepository<TEntity>
                                                   where TEntity : class;

    /// <summary>
    /// Begin a context transaction
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    void BeginTransaction(CancellationToken cancellationToken = default);

    /// <summary>
    /// Do the context commit
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>An identifier value</returns>
    int Commit(CancellationToken cancellationToken = default);

    /// <summary>
    /// Do the context commit async
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>An identifier value</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the context transaction
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    void Rollback(CancellationToken cancellationToken = default);
}
