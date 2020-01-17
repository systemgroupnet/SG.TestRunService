using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SG.TestRunService.Infrastructure;

namespace SG.TestRunService.Data.Services.Implementations
{
    public class EntityDbServiceBase<TEntity> : IEntityDbService<TEntity>
        where TEntity : class, IEntity
    {
        protected TSDbContext Db { get; private set; }
        protected IBaseDbService BaseDbService { get; set; }

        public EntityDbServiceBase(TSDbContext db, IBaseDbService baseDbService)
        {
            Db = db;
            BaseDbService = baseDbService;
        }

        public Task<IReadOnlyList<T>> GetAllAsync<T>(Expression<Func<TEntity, T>> projection)
        {
            return BaseDbService.GetAllAsync(projection);
        }

        public Task<IReadOnlyList<T>> GetAllAsync<T>(Func<IQueryable<TEntity>, IQueryable<T>> projector)
        {
            return BaseDbService.GetAllAsync(projector);
        }

        public Task<IReadOnlyList<T>> GetFilteredAsync<T>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, T>> projection)
        {
            return BaseDbService.GetFilteredAsync(filter, projection);
        }

        public Task<IReadOnlyList<T>> GetFilteredAsync<T>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<T>> projector)
        {
            return BaseDbService.GetFilteredAsync(filter, projector);
        }

        public Task<T> GetFirstOrDefaultAsync<T>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<T>> projector)
        {
            return BaseDbService.GetFirstOrDefaultAsync(filter, projector);
        }

        public Task<T> GetByIdAsync<T>(int id, Func<IQueryable<TEntity>, IQueryable<T>> projector)
        {
            return BaseDbService.GetByIdAsync(id, projector);
        }

        public Task InsertAsync(TEntity entity)
        {
            return BaseDbService.InsertAsync(entity);
        }

        public Task UpdateAsync(TEntity entity)
        {
            return BaseDbService.UpdateAsync(entity);
        }

        public Task DeleteAsync(TEntity entity)
        {
            return BaseDbService.DeleteAsync(entity);
        }

        public Task<TEntity> DeleteAsync(int id)
        {
            return BaseDbService.DeleteAsync<TEntity>(id);
        }
    }
}

