namespace ReconNess.Infrastructure.Data.EF.Npgsql;

using ReconNess.Application.DataAccess;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// This class implement <see cref="IUnitOfWork"/>
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    /// <summary>
    /// The DataBase Context
    /// </summary>
    private readonly IDbContext context;

    /// <summary>
    /// A hash of repositories
    /// </summary>
    private Hashtable repositories;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork" /> class
    /// </summary>
    /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
    public UnitOfWork(IDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public IRepository<TEntity> Repository<TEntity>(CancellationToken cancellationToken = default)
        where TEntity : class
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (repositories == null)
        {
            repositories = new Hashtable();
        }

        var type = typeof(TEntity).Name;

        if (repositories.ContainsKey(type))
        {
            return (IRepository<TEntity>)repositories[type];
        }

        var repositoryType = typeof(Repository<TEntity>);

        repositories.Add(type, Activator.CreateInstance(repositoryType, context));

        return (IRepository<TEntity>)repositories[type];
    }

    /// <inheritdoc/>
    public void BeginTransaction(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        context.BeginTransaction(cancellationToken);
    }

    /// <inheritdoc/>
    public int Commit(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return context.Commit(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return context.CommitAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public void Rollback(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        context.Rollback(cancellationToken);
    }
}
