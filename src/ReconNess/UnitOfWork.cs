namespace ReconNess
{
    using ReconNess.Core;
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

            if (this.repositories == null)
            {
                this.repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;

            if (this.repositories.ContainsKey(type))
            {
                return (IRepository<TEntity>)this.repositories[type];
            }

            var repositoryType = typeof(Repository<TEntity>);

            this.repositories.Add(type, Activator.CreateInstance(repositoryType, this.context));

            return (IRepository<TEntity>)this.repositories[type];
        }

        /// <inheritdoc/>
        public void BeginTransaction(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.BeginTransaction(cancellationToken);
        }

        /// <inheritdoc/>
        public int Commit(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.Commit(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.CommitAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public void Rollback(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.Rollback(cancellationToken);
        }
    }
}
