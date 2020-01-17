using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SG.TestRunService.Infrastructure
{
    public interface IBaseDbService
    {
        Task<IReadOnlyList<TOutput>> GetAllAsync<TEntity, TOutput>(Expression<Func<TEntity, TOutput>> projection) where TEntity : class, IEntity;
        Task<IReadOnlyList<TOutput>> GetAllAsync<TEntity, TOutput>(Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class, IEntity;
        Task<IReadOnlyList<TOutput>> GetFilteredAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TOutput>> projection) where TEntity : class, IEntity;
        Task<IReadOnlyList<TOutput>> GetFilteredAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class, IEntity;
        Task<TOutput> GetFirstOrDefaultAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TOutput>> projection) where TEntity : class, IEntity;
        Task<TOutput> GetFirstOrDefaultAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class, IEntity;
        Task<TOutput> GetByIdAsync<TEntity, TOutput>(int id, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class, IEntity;
        Task<TEntity> GetByIdAsync<TEntity>(int id) where TEntity : class, IEntity;
        Task InsertAsync<TEntity>(TEntity entity) where TEntity : class, IEntity;
        Task InsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;
        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity;
        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity;
        Task<TEntity> DeleteAsync<TEntity>(int id) where TEntity : class, IEntity;
        IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity;
        IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class, IEntity;
        IQueryable<TEntity> Query<TEntity>(int id) where TEntity : class, IEntity;
        IQueryable<TEntity> Query<TEntity>(IEnumerable<int> ids) where TEntity : class, IEntity;
    }
}
