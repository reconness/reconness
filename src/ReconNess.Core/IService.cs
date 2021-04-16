namespace ReconNess.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The interface IService<TEntity>
    /// </summary>
    /// <typeparam name="TEntity">An Entity</typeparam>
    public interface IService<TEntity>
    {
        /// <summary>
        /// Obtain the Entity using cache
        /// </summary>
        /// <param name="id">The entity Id</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>Entity by id</returns>
        Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain the Entity using cache
        /// </summary>
        /// <param name="predicate">The criteria</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>Entity by id</returns>
        Task<TEntity> FindByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain the async list of generic Entities by a criteria
        /// </summary>
        /// <param name="predicate">The criteria</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The async list of generic Entities</returns>
        Task<TEntity> GetByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain the async list of generic Entities
        /// </summary>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The async list of generic Entities</returns>
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain if exist some data on BD using that predicate
        /// </summary>
        /// <param name="predicate">The criteria</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>If exist some data on BD using that predicate</returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtain the async queryable of generic Entities
        /// </summary>
        /// <returns>The async queryable of generic Entities</returns>
        IQueryable<TEntity> GetAllQueryable();

        /// <summary>
        ///  Obtain the async list of generic Entities by criteria
        /// </summary>
        /// <param name="predicate">The criteria</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The async list of generic Entities by criteria</returns>
        Task<List<TEntity>> GetAllByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Obtain the async queryable of generic Entities by criteria
        /// </summary>
        /// <param name="predicate">The criteria</param>
        /// <returns>The async queryable of generic Entities by criteria</returns>
        IQueryable<TEntity> GetAllQueryableByCriteria(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Add a new Entity into the repository and do a commit
        /// </summary>
        /// <param name="entity">The generic Entity</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The new Entity</returns>
        TEntity Add(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add a new list of Entities into the repository and do a commit
        /// </summary>
        /// <param name="entities">The list of Entities</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of Entities</returns>
        List<TEntity> AddRange(List<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add a new Entity async into the repository and do a commit
        /// </summary>
        /// <param name="entity">The generic Entity</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The new Entity</returns>
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add a new list of Entities async into the repository and do a commit
        /// </summary>
        /// <param name="entity">The list of Entities</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of Entities</returns>
        Task<List<TEntity>> AddRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an Entity into the repository and do a commit
        /// </summary>
        /// <param name="entity">The generic Entity</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The updated Entity</returns>
        TEntity Update(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a list of Entities into the repository and do a commit
        /// </summary>
        /// <param name="entities">The list of Entities</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of Entities</returns>
        List<TEntity> UpdateRange(List<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an Entity async into the repository and do a commit
        /// </summary>
        /// <param name="entity">The generic Entity</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The updated Entity</returns>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a list of Entities async into the repository and do a commit
        /// </summary>
        /// <param name="entities">The list of Entities</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The list of Entities</returns>
        Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an Entity into the repository and do a commit
        /// </summary>
        /// <param name="entity">The generic Entity</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        void Delete(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a list of Entitiesinto the repository and do a commit
        /// </summary>
        /// <param name="entities">>The list of Entities</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        void DeleteRange(List<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an Entity async into the repository and do a commit
        /// </summary>
        /// <param name="entity">The generic Entity</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>The deleted Entity</returns>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a list of Entities async into the repository and do a commit
        /// </summary>
        /// <param name="entities">>The list of Entities</param>
        /// <param name="cancellationToken">Notification that operations should be canceled</param>
        /// <returns>A list of Entities async into the repository and do a commit</returns>
        Task DeleteRangeAsync(List<TEntity> entities, CancellationToken cancellationToken = default);
    }
}
