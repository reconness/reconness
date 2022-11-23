namespace ReconNess.Application.DataAccess;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The interface IRepository<TEntity>
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Obtain the async list of Entities
    /// </summary>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>Async list of Entities</returns>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain the async queryable of generic Entities
    /// </summary>
    /// <returns>The async queryable of generic Entities</returns>
    IQueryable<TEntity> GetAllQueryable();

    /// <summary>
    /// Obtain the async list of Entities
    /// </summary>
    /// <param name="predicate">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>The async list of generic Entities by criteria</returns>
    Task<List<TEntity>> GetAllByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain if exist some data on BD using that predicate
    /// </summary>
    /// <param name="predicate">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>If exist some data on BD using that predicate</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain the async queryable of generic Entities by criteria
    /// </summary>
    /// <param name="predicate">The criteria</param>
    /// <returns>The async queryable of generic Entities by criteria</returns>
    IQueryable<TEntity> GetAllQueryableByCriteria(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Obtain the async list of Entities by criteria
    /// </summary>
    /// <param name="predicate">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>Async list of Entities by criteria</returns>
    Task<TEntity> GetByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain the entity by Id
    /// </summary>
    /// <param name="id">The entity Id</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>An Entity</returns>
    Task<TEntity> FindAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtain the entity by Id
    /// </summary>
    /// <param name="predicate">The criteria</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    /// <returns>An Entity</returns>
    Task<TEntity> FindByCriteriaAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new Entity into the context
    /// </summary>
    /// <param name="entity">The new Entity</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    void Add(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///  Add a list of Entities into the context
    /// </summary>
    /// <param name="entities">List of new Entities</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    void AddRange(List<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an Entity
    /// </summary>
    /// <param name="entity">The Entity</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    void Update(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a List of Entities
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    void UpdateRange(List<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an Entity
    /// </summary>
    /// <param name="entity">The Entity</param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    void Delete(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a list of Entities
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="cancellationToken">Notification that operations should be canceled</param>
    void DeleteRange(List<TEntity> entities, CancellationToken cancellationToken = default);
}
