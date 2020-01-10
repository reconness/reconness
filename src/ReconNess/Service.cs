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

        /// <summary>
        /// Gets or Sets the Unit Of Work pattern <see cref="IUnitOfWork" />
        /// </summary>
        public IUnitOfWork UnitOfWork { get; private set; }

        /// <summary>
        /// <see cref="IService{TEntity}.FindAsync(Guid, CancellationToken)"/>
        /// </summary>
        public Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.FindAsync(id, cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.FindByCriteriaAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<TEntity> FindByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.FindByCriteriaAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.GetAllAsync(CancellationToken)"/>
        /// </summary>
        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.GetAllAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.AnyAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.AnyAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.GetAllQueryable(CancellationToken)"/>
        /// </summary>
        public IQueryable<TEntity> GetAllQueryable(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.GetAllQueryable(cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.GetAllByCriteriaAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<List<TEntity>> GetAllByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.GetAllByCriteriaAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.GetAllQueryableByCriteria(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public IQueryable<TEntity> GetAllQueryableByCriteria(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.GetAllQueryableByCriteria(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.GetByCriteriaAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public Task<TEntity> GetByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return this.repository.GetByCriteriaAsync(predicate, cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.Add(TEntity, CancellationToken)"/>
        /// </summary>
        public TEntity Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Add(entity, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);

            return entity;
        }

        /// <summary>
        /// <see cref="IService{TEntity}.AddRange(List{TEntity}, CancellationToken)"/>
        /// </summary>
        public List<TEntity> AddRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.AddRange(entities, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);

            return entities;
        }

        /// <summary>
        /// <see cref="IService{TEntity}.AddAsync(TEntity, CancellationToken)"/>
        /// </summary>
        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Add(entity, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);

            return entity;
        }

        /// <summary>
        /// <see cref="IService{TEntity}.AddRangeAsync(List{TEntity}, CancellationToken)"/>
        /// </summary>
        public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.AddRange(entities, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);

            return entities;
        }

        /// <summary>
        /// <see cref="IService{TEntity}.Update(TEntity, CancellationToken)"/>
        /// </summary>
        public TEntity Update(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Update(entity, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);

            return entity;
        }

        /// <summary>
        /// <see cref="IService{TEntity}.UpdateRange(List{TEntity}, CancellationToken)"/>
        /// </summary>
        public List<TEntity> UpdateRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.UpdateRange(entities, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);

            return entities;
        }

        /// <summary>
        /// <see cref="IService{TEntity}.UpdateAsync(TEntity, CancellationToken)"/>
        /// </summary>
        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Update(entity, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);

            return entity;
        }

        /// <summary>
        /// <see cref="IService{TEntity}.UpdateRangeAsync(List{TEntity}, CancellationToken)"/>
        /// </summary>
        public async Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.UpdateRange(entities, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);

            return entities;
        }

        /// <summary>
        /// <see cref="IService{TEntity}.Delete(TEntity, CancellationToken)"/>
        /// </summary>
        public void Delete(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Delete(entity, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.DeleteRange(List{TEntity}, CancellationToken)"/>
        /// </summary>
        public void DeleteRange(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.DeleteRange(entities, cancellationToken);
            this.UnitOfWork.Commit(cancellationToken);
        }

        /// <summary>
        /// <see cref="IService{TEntity}.DeleteAsync(TEntity, CancellationToken)"/>
        /// </summary>
        public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.Delete(entity, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);
        }

        /// <summary>
        ///  <see cref="IService{TEntity}.DeleteRangeAsync(List{TEntity}, CancellationToken)"/>
        /// </summary>
        public async Task DeleteRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.repository.DeleteRange(entities, cancellationToken);
            await this.UnitOfWork.CommitAsync(cancellationToken);
        }
    }
}
