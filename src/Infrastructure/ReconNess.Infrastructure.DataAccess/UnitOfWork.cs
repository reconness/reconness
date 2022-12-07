namespace ReconNess.Infrastructure.DataAccess;

using ReconNess.Application.DataAccess;
using ReconNess.Application.DataAccess.Repositories;
using ReconNess.Infrastructure.DataAccess.Repositories;
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
    private Hashtable? repositories;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork" /> class
    /// </summary>
    /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
    public UnitOfWork(IDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public IRepository<TEntity> Repository<TEntity>()
        where TEntity : class
    {
        if (repositories == null)
        {
            repositories = new Hashtable();
        }

        var type = typeof(TEntity).Name;

        if (repositories.ContainsKey(type))
        {
            return (IRepository<TEntity>)repositories[type]!;
        }

        var repositoryType = typeof(Repository<TEntity>);

        repositories.Add(type, Activator.CreateInstance(repositoryType, context));

        return (IRepository<TEntity>)repositories[type]!;
    }

    /// <inheritdoc/>
    public TCustomRepo Repository<TCustomRepo, TEntity>()
        where TCustomRepo : IRepository<TEntity>
        where TEntity : class
    {
        if (repositories == null)
        {
            repositories = new Hashtable();
        }

        var type = typeof(TCustomRepo).Name;

        if (repositories.ContainsKey(type))
        {
            return (TCustomRepo)repositories[type]!;
        }

        var repositoryType = GetCustomRepository(type);
        repositories.Add(type, Activator.CreateInstance(repositoryType, context));

        return (TCustomRepo)repositories[type]!;
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

    /// <summary>
    /// Obtain the customer repository base in the <see cref="TCustomRepo"/>
    /// </summary>
    /// <param name="type">The custom repository type</param>
    /// <returns>The customer repository base in the <see cref="type"/></returns>
    /// <exception cref="InvalidOperationException">If invalid repository</exception>
    private static Type GetCustomRepository(string type) => type switch
    {
        nameof(IAgentRepository) => typeof(AgentRepository),
        nameof(IAgentRunnerRepository) => typeof(AgentRunnerRepository),
        nameof(IReferenceRepository) => typeof(ReferenceRepository),
        nameof(IRootDomainRepository) => typeof(RootDomainRepository),
        nameof(ISubdomainRepository) => typeof(SubdomainRepository),
        nameof(ITargetRepository) => typeof(TargetRepository),
        nameof(IDirectoryRepository) => typeof(DirectoryRepository),        
        _ => throw new InvalidOperationException($"Invalid repository {type}")
    };    
}
