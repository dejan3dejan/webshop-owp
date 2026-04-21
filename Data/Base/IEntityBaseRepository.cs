using System.Linq.Expressions;

namespace webshop_owp.Data.Base
{
    /// <summary>
    /// Defines a generic repository pattern for standard CRUD operations on entities.
    /// </summary>
    /// <typeparam name="T">The entity type that implements IEntityBase.</typeparam>
    public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        /// <summary>
        /// Retrieves all entities from the database without any related data.
        /// </summary>
        /// <returns>A collection containing all entities of the specified type.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retrieves all entities from the database, eagerly loading the specified related properties.
        /// </summary>
        /// <param name="includeProperties">A variable-length list of navigation properties to include in the query.</param>
        /// <returns>A collection containing all entities with the specified related data included.</returns>
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Retrieves a single entity by its primary identifier without any related data.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Retrieves a single entity by its primary identifier, eagerly loading the specified related properties.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="includeProperties">A variable-length list of navigation properties to include in the query.</param>
        /// <returns>The entity with the specified related data included if found; otherwise, null.</returns>
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Adds a new entity to the database and saves changes.
        /// </summary>
        /// <param name="entity">The entity to persist.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity in the database and saves changes.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to update.</param>
        /// <param name="entity">The entity instance containing the updated values.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task UpdateAsync(int id, T entity);

        /// <summary>
        /// Marks an entity as deleted in the database and saves changes.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task DeleteAsync(int id);
    }
}
