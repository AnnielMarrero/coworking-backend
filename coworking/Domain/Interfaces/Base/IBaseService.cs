using coworking.Dtos.Base;
using coworking.Entities;
using EntityFrameworkPaginateCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace coworking.Domain.Interfaces
{
    public interface IBaseService<TEntity> where TEntity : BasicEntity
    {

        EntityEntry<TEntity> Update(TEntity entity);
        /// <summary>
        /// Remove an entity from database
        /// </summary>
        EntityEntry<TEntity> Delete(TEntity entity);
        /// <summary>
        /// Update many entities from database
        /// </summary>
        void UpdateRange(List<TEntity> entities);
        /// <summary>
        /// Find an entity by Id
        /// </summary>
        Task<TEntity?> FindByIdAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties);
        /// <summary>
        /// Firsrt or default with includes
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includeProperties);
        /// <summary>
        /// Save All Changes in database
        /// </summary>
        Task<int> SaveChangesAsync();
        /// <summary>
        /// Get entities list paginated
        /// </summary>
        Task<Page<TEntity>> GetPagedListAsync(PagedListInputDto inputDto, params Expression<Func<TEntity, bool>>[] filters);
        /// <summary>
        /// Get all entities 
        /// </summary>

        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includeProperties);

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includeProperties);
        /// <summary>
        /// Add an entity
        /// </summary>
        Task<EntityEntry<TEntity>> AddAsync(TEntity entity);
        Task<TEntity> UpdateModel(TEntity entity);
        //Task<EntityEntry<TEntity>?> DeleteAsync(TEntity entity);

        /// <summary>
        /// Add many entities
        /// </summary>
        /// <param name="entities">Entities a agregar</param>
        Task AddRangeAsync(List<TEntity> entities);


        Task<bool> ValidateAdd(TEntity entity);
        Task<bool> ValidateEdit(TEntity entity);
        Task<bool> ValidateRemove(int id);

        Task SetHistory(string? user, string descripcion, string tablaBD, object elemento, object? elementoModificado = null);
    }
}