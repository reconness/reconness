﻿namespace ReconNess
{
    using ReconNess.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class implement <see cref="IRepository<TEntity>"/>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// The DataBase Context
        /// </summary>
        private readonly IDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}" /> class
        /// </summary>
        /// <param name="context">The implementation of Database Context <see cref="IDbContext" /></param>
        public Repository(IDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.ToListAsync<TEntity>(cancellationToken);
        }

        /// <inheritdoc/>
        public IQueryable<TEntity> GetAllQueryable()
        {
            return this.context.ToQueryable<TEntity>();
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> GetAllByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.ToListByCriteriaAsync<TEntity>(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.AnyAsync<TEntity>(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public IQueryable<TEntity> GetAllQueryableByCriteria(Expression<Func<TEntity, bool>> predicate)
        {
            return this.context.ToQueryableByCriteria<TEntity>(predicate);
        }

        /// <inheritdoc/>
        public Task<TEntity> GetByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.FirstOrDefaultAsync<TEntity>(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> FindByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.FindByCriteriaAsync<TEntity>(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.FindAsync<TEntity>(id, cancellationToken);
        }

        /// <inheritdoc/>
        public void Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsAdded<TEntity>(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public void AddRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsAdded<TEntity>(entities, cancellationToken);
        }

        /// <inheritdoc/>
        public void Update(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsModified<TEntity>(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public void UpdateRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsModified<TEntity>(entities, cancellationToken);
        }

        /// <inheritdoc/>
        public void Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsDeleted<TEntity>(entity, cancellationToken);
        }

        /// <inheritdoc/>
        public void DeleteRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsDeleted<TEntity>(entities, cancellationToken);
        }
    }
}
