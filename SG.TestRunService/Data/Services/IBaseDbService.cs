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
        Task<IReadOnlyList<TOutput>> GetAllAsync<TEntity, TOutput>(Expression<Func<TEntity, TOutput>> projection) where TEntity : class;
        Task<IReadOnlyList<TOutput>> GetAllAsync<TEntity, TOutput>(Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class;
        Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>() where TEntity : class;
        Task<IReadOnlyList<TOutput>> GetFilteredAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TOutput>> projection) where TEntity : class;
        Task<IReadOnlyList<TOutput>> GetFilteredAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class;
        Task<IReadOnlyList<TEntity>> GetFilteredAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        Task<TOutput> GetFirstOrDefaultAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TOutput>> projection) where TEntity : class;
        Task<TOutput> GetFirstOrDefaultAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class;
        Task<TEntity> GetFirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        Task<TOutput> GetByIdAsync<TEntity, TOutput>(int id, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector) where TEntity : class, IEntity;
        Task<TEntity> GetByIdAsync<TEntity>(int id) where TEntity : class, IEntity;
        Task InsertAsync<TEntity>(TEntity entity) where TEntity : class;
        Task InsertAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<TEntity> DeleteAsync<TEntity>(int id) where TEntity : class, IEntity;
        IQueryable<TEntity> Query<TEntity>() where TEntity : class;
        IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;
        IQueryable<TEntity> Query<TEntity>(int id) where TEntity : class, IEntity;
        IQueryable<TEntity> Query<TEntity>(IEnumerable<int> ids) where TEntity : class, IEntity;

        void Add<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
        Task<int> SaveChangesAsync();
    }
}
