using coworking.Domain.Interfaces;
using coworking.Dtos.Base;
using coworking.Entities;
using coworking.UnitOfWork.Interfaces.Base;
using EntityFrameworkPaginateCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq.Expressions;

namespace coworking.Domain.Services.Base
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BasicEntity
    {
        protected readonly IBaseRepository<TEntity> _baseRepository;
        protected readonly IUnitOfWork _repositories;
        protected readonly IHttpContextAccessor _httpContext;
        private readonly string userName;

        public BaseService(IUnitOfWork repositories, IBaseRepository<TEntity> baseRepository, IHttpContextAccessor httpContext)
        {
            _baseRepository = baseRepository;
            _repositories = repositories;
            _httpContext = httpContext;
            userName = _httpContext.HttpContext?.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value ?? string.Empty;
        }

        public virtual async Task<EntityEntry<TEntity>> AddAsync(TEntity entity) => await _baseRepository.AddAsync(SetAuditData(entity));
        public virtual async Task AddRangeAsync(List<TEntity> entities) => await _baseRepository.AddRangeAsync(SetAuditData(entities));
        public virtual EntityEntry<TEntity> Delete(TEntity entity) => _baseRepository.Remove(entity);
        //public virtual async Task<EntityEntry<TEntity>?> DeleteAsync(TEntity entity)
        //{   
        //    return _baseRepository.Remove(entity);
        //}
        public virtual async Task<int> SaveChangesAsync() => await _baseRepository.SaveChangesAsync();
        public virtual EntityEntry<TEntity> Update(TEntity entity) => _baseRepository.Update(SetAuditData(entity, false));
        public virtual void UpdateRange(List<TEntity> entities) => _baseRepository.UpdateRange(SetAuditData(entities, false));
        public virtual async Task<TEntity?> FindByIdAsync(int id, params Expression<Func<TEntity, object>>[] includeProperties) => await _baseRepository.FirstOrDefaultAsync(entity => entity.Id == id, includeProperties);

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includeProperties) => await _baseRepository.FirstOrDefaultAsync(condition, includeProperties);

        protected virtual IQueryable<TEntity> CreateQuery()
        {
            return _baseRepository.GetQuery();
        }

        public virtual async Task<Page<TEntity>> GetPagedListAsync(PagedListInputDto inputDto, params Expression<Func<TEntity, bool>>[] filters)
        {
            IQueryable<TEntity> query = CreateQuery();

            //apply custom filters, si son especificados en cada clase Servicio q sobrescribe este metodo
            query = filters.Aggregate(query, (current, filters) => current.Where(filters));


            //Paginating
            var result = await query.PaginateAsync(inputDto.Page ?? 1, inputDto.PageSize ?? inputDto.DefaultPageSize);
            return result;
        }

        public Task<TEntity> UpdateModel(TEntity entity)
        {
            return _baseRepository.UpdateModel(entity);
        }

        public virtual async Task<bool> ValidateAdd(TEntity entity)
        {
            return await Task.Run(() => true);
        }

        public virtual async Task<bool> ValidateEdit(TEntity entity)
        {
            return await Task.Run(() => true);
        }

        public virtual async Task<bool> ValidateRemove(int id)
        {
            return await Task.Run(() => true);
        }
        /// <summary>
        /// Sets the values for the entity's auditable data
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isNewElement"></param>
        /// <returns></returns>
        private TEntity SetAuditData(TEntity entity, bool isNewElement = true)
        {
            if (isNewElement)
            {
                entity.CreatedAt = DateTime.Now;
                //entity.CreatedBy = userName;
            }
            entity.UpdatedAt = DateTime.Now;
            //entity.UpdatedBy = userName;
            return entity;
        }

        public async Task SetHistory(string? user, string description, string table, object element, object? updateElement = null)
        {
            Log history = new Log()
            {
                Description = description,
                Table = table,
                Element = JsonConvert.SerializeObject(element),
                UpdateElement = updateElement == null ? string.Empty : JsonConvert.SerializeObject(updateElement),
                CreatedAt = DateTime.Now,
                CreatedBy = user,
                UpdatedAt = DateTime.Now,
                UpdatedBy = user
            };
            await _repositories.Logs.AddAsync(history);
            await SaveChangesAsync();

        }

        private List<TEntity> SetAuditData(List<TEntity> entities, bool isNewElement = true)
        {
            entities.ForEach(entity => SetAuditData(entity, isNewElement));
            return entities;
        }



        public async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includeProperties) =>
            await _baseRepository.GetAllAsync(includeProperties);

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> condition, params Expression<Func<TEntity, object>>[] includeProperties) =>
            await _baseRepository.GetAllAsync(condition, includeProperties);

        /*
        private static string BuildOrder(PagedListInputDto inputDto)
        {
            string order = inputDto.OrderByCriteria == "asc" ? "asc" : "desc";
            return $"{inputDto.OrderByProp ?? "Id"} {order}";
        }

        private static string BuildFilter(PagedListInputDto inputDto)
        {
            if (string.IsNullOrEmpty(inputDto.SearchByProp) || string.IsNullOrEmpty(inputDto.SearchCriteria)) return "";
            return $"{inputDto.SearchByProp}=*{inputDto.SearchCriteria}/i"; //i make insensite the search, * means contains
        }
        */
    }

}