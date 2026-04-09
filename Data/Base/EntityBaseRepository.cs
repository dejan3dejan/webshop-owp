using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace webshop_owp.Data.Base
{
    /// <summary>
    /// Provides a generic repository implementation for standard entity database operations using Entity Framework Core.
    /// </summary>
    /// <typeparam name="T">The type of the entity, which must implement IEntityBase.</typeparam>
    public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        protected readonly AppDbContext _context;
        public EntityBaseRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new entity to the database and saves changes.
        /// </summary>
        /// <param name="entity">The entity instance to add.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Marks an entity as deleted in the database and saves changes.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);
            if (entity == null) return;
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = EntityState.Deleted;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all entities of the specified type from the database.
        /// </summary>
        /// <returns>A collection containing all entities.</returns>
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        /// <summary>
        /// Retrieves all entities from the database, eagerly loading the specified navigation properties.
        /// </summary>
        /// <param name="includeProperties">A variable-length list of navigation properties to eagerly load.</param>
        /// <returns>A collection containing all entities with the related properties included.</returns>
        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.ToListAsync();
        }

        /// <summary>
        /// Retrieves a single entity by its primary identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The specified entity, or null if not found.</returns>
        public async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);

        /// <summary>
        /// Retrieves a single entity by its primary identifier, eagerly loading the specified navigation properties.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="includeProperties">A variable-length list of navigation properties to eagerly load.</param>
        /// <returns>The specified entity with related properties included, or null if not found.</returns>
        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// Updates the state of an existing entity in the database and saves changes.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to update.</param>
        /// <param name="entity">The entity instance with updated values.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        public async Task UpdateAsync(int id, T entity)
        {
            if (entity.Id != id) throw new ArgumentException($"Entity id {entity.Id} does not match requested id {id}.");
            EntityEntry entityEntry = _context.Entry<T>(entity);
            entityEntry.State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
