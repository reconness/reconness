namespace ReconNess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using ReconNess.Core;

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

        /// <summary>
        /// <see cref="IRepository{TEntity}.GetAllAsync(CancellationToken)"/>
        /// </summary>
        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.ToListAsync<TEntity>(cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.GetAllQueryable(CancellationToken)"/>
        /// </summary>
        public IQueryable<TEntity> GetAllQueryable(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.ToQueryable<TEntity>(cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.GetAllByCriteriaAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<List<TEntity>> GetAllByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.ToListByCriteriaAsync<TEntity>(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.AnyAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.AnyAsync<TEntity>(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.GetAllQueryableByCriteria(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public IQueryable<TEntity> GetAllQueryableByCriteria(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.ToQueryableByCriteria<TEntity>(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.GetByCriteriaAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<TEntity> GetByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.FirstOrDefaultAsync<TEntity>(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.FindByCriteriaAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<TEntity> FindByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.FindByCriteriaAsync<TEntity>(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.FindAsync(Guid, CancellationToken)"/>
        /// </summary>
        public Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.context.FindAsync<TEntity>(id, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.Add(TEntity, CancellationToken)/>
        /// </summary>
        public void Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsAdded<TEntity>(entity, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.AddRange(List{TEntity}, CancellationToken)"/
        /// </summary>
        public void AddRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsAdded<TEntity>(entities, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.Update(TEntity, CancellationToken)"/>
        /// </summary>
        public void Update(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsModified<TEntity>(entity, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.UpdateRange(List{TEntity}, CancellationToken)"/>
        /// </summary>
        public void UpdateRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsModified<TEntity>(entities, cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity}.Delete(TEntity, CancellationToken)"/>
        /// </summary>
        public void Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsDeleted<TEntity>(entity, cancellationToken);
        }

        /// <summary>
        ///  <see cref="IRepository{TEntity}.DeleteRange(List{TEntity}, CancellationToken)"/>
        /// </summary>
        public void DeleteRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.context.SetAsDeleted<TEntity>(entities, cancellationToken);
        }
    }
}
