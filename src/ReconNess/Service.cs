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
    /// This class implement the interface <see cref="IService<TEntity>"/>
    /// </summary>
    /// <typeparam name="TEntity">An Entity</typeparam>
    public class Service<TEntity> : IService<TEntity> where TEntity : class
    {
        /// <summary>
        /// The generic repository <see cref="IRepository{TEntity}"/>
        /// </summary>
        private readonly IRepository<TEntity> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="Service{TEntity}" /> class
        /// </summary>
        /// <param name="unitOfWork">The implementation of Unit Of Work pattern <see cref="IUnitOfWork" /></param>
        public Service(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
            this.repository = this.UnitOfWork.Repository<TEntity>();
        }

        /// <inheritdoc/>
        public IUnitOfWork UnitOfWork { get; private set; }

        /// <inheritdoc/>
        public Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.FindAsync(id, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<TEntity> FindByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.FindByCriteriaAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.GetAllAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.AnyAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public IQueryable<TEntity> GetAllQueryable()
        {
            return this.repository.GetAllQueryable();
        }

        /// <inheritdoc/>
        public Task<List<TEntity>> GetAllByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.GetAllByCriteriaAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public IQueryable<TEntity> GetAllQueryableByCriteria(Expression<Func<TEntity, bool>> predicate)
        {
            return this.repository.GetAllQueryableByCriteria(predicate);
        }

        /// <inheritdoc/>
        public Task<TEntity> GetByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.GetByCriteriaAsync(predicate, cancellationToken);
        }

        /// <inheritdoc/>
        public TEntity Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Add(entity, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);

            return entity;
        }

        /// <inheritdoc/>
        public List<TEntity> AddRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.AddRange(entities, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);

            return entities;
        }

        /// <inheritdoc/>
        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Add(entity, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);

            return entity;
        }

        /// <inheritdoc/>
        public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.AddRange(entities, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);

            return entities;
        }

        /// <inheritdoc/>
        public TEntity Update(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Update(entity, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);

            return entity;
        }

        /// <inheritdoc/>
        public List<TEntity> UpdateRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.UpdateRange(entities, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);

            return entities;
        }

        /// <inheritdoc/>
        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Update(entity, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);

            return entity;
        }

        /// <inheritdoc/>
        public async Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.UpdateRange(entities, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);

            return entities;
        }

        /// <inheritdoc/>
        public void Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Delete(entity, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);
        }

        /// <inheritdoc/>
        public void DeleteRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.DeleteRange(entities, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Delete(entity, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task DeleteRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.DeleteRange(entities, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);
        }
    }
}
