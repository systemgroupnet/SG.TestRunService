using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SG.TestRunService.Data.Services
{
    public interface IBaseDbService
    {
        Task<IReadOnlyList<TOutput>> GetAllAsync<TEntity, TOutput>(Expression<Func<TEntity, TOutput>> projection) where TEntity : class, IEntity;
        Task<IReadOnlyList<TOutput>> GetAllAsync<TEntity, TOutput>(Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class, IEntity;
        Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>() where TEntity : class, IEntity;
        Task<IReadOnlyList<TOutput>> GetFilteredAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TOutput>> projection) where TEntity : class, IEntity;
        Task<IReadOnlyList<TOutput>> GetFilteredAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class, IEntity;
        Task<IReadOnlyList<TEntity>> GetFilteredAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class, IEntity;
        Task<TOutput> GetFirstOrDefaultAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TOutput>> projection) where TEntity : class, IEntity;
        Task<TOutput> GetFirstOrDefaultAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class, IEntity;
        Task<TEntity> GetFirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class, IEntity;
        Task<TOutput> GetByIdAsync<TEntity, TKey, TOutput>(TKey id, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class, IEntity<TKey>;
        Task<TEntity> GetByIdAsync<TEntity, TKey>(TKey id) where TEntity : class, IEntity<TKey>;
        Task InsertAsync<TEntity>(TEntity entity) where TEntity : class, IEntity;
        Task InsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;
        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity;
        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IEntity;
        Task<TEntity> DeleteAsync<TEntity, TKey>(TKey id) where TEntity : class, IEntity<TKey>;
        Task<TEntity> DeleteAsync<TEntity>(int id) where TEntity : class, IEntity<int>;
        IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity;
        IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class, IEntity;
        IQueryable<TEntity> Query<TEntity, TKey>(TKey id) where TEntity : class, IEntity<TKey>;
        IQueryable<TEntity> Query<TEntity, TKey>(IEnumerable<TKey> ids) where TEntity : class, IEntity<TKey>;
        IQueryable<TEntity> Query<TEntity>(int id) where TEntity : class, IEntity<int>;
        IQueryable<TEntity> Query<TEntity>(IEnumerable<int> ids) where TEntity : class, IEntity<int>;

        void Add<TEntity>(TEntity entity) where TEntity : class, IEntity;
        void Remove<TEntity>(TEntity entity) where TEntity : class, IEntity;
        Task<int> SaveChangesAsync();
        void ResetDbContext();
    }
}
