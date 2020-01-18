using Microsoft.EntityFrameworkCore;
using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SG.TestRunService.Data.Services.Implementations
{
    public sealed class BaseDbService : IBaseDbService
    {
        private readonly TSDbContext _db;

        public BaseDbService(TSDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<TOutput>> GetAllAsync<TEntity, TOutput>(Expression<Func<TEntity, TOutput>> projection)
            where TEntity : class, IEntity
        {
            return await _db.Set<TEntity>().Select(projection).ToListAsync();
        }

        public async Task<IReadOnlyList<TOutput>> GetAllAsync<TEntity, TOutput>(Func<IQueryable<TEntity>, IQueryable<TOutput>> projector)
            where TEntity : class, IEntity
        {
            return await projector(_db.Set<TEntity>()).ToListAsync();
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync<TEntity>()
            where TEntity : class, IEntity
        {
            return await _db.Set<TEntity>().ToListAsync();
        }

        public async Task<IReadOnlyList<TOutput>> GetFilteredAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TOutput>> projection)
            where TEntity : class, IEntity
        {
            return await _db.Set<TEntity>().Where(filter).Select(projection).ToListAsync();
        }

        public async Task<IReadOnlyList<TOutput>> GetFilteredAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector)
            where TEntity : class, IEntity
        {
            return await projector(_db.Set<TEntity>().Where(filter)).ToListAsync();
        }

        public async Task<IReadOnlyList<TEntity>> GetFilteredAsync<TEntity>(Expression<Func<TEntity, bool>> filter)
            where TEntity : class, IEntity
        {
            return await _db.Set<TEntity>().Where(filter).ToListAsync();
        }

        public async Task<TOutput> GetFirstOrDefaultAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TOutput>> projection)
            where TEntity : class, IEntity
        {
            return await _db.Set<TEntity>().Where(filter).Select(projection).FirstOrDefaultAsync();
        }

        public async Task<TOutput> GetFirstOrDefaultAsync<TEntity, TOutput>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector)
            where TEntity : class, IEntity
        {
            return await projector(_db.Set<TEntity>().Where(filter)).FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetFirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter)
            where TEntity : class, IEntity
        {
            return await _db.Set<TEntity>().Where(filter).FirstOrDefaultAsync();
        }

        public Task<TOutput> GetByIdAsync<TEntity, TOutput>(int id, Func<IQueryable<TEntity>, IQueryable<TOutput>> projector)
            where TEntity : class, IEntity
        {
            return GetFirstOrDefaultAsync(e => e.Id == id, projector);
        }

        public Task<TEntity> GetByIdAsync<TEntity>(int id) where TEntity : class, IEntity
        {
            return _db.FindAsync<TEntity>(id).AsTask();
        }

        public async Task InsertAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            _db.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task InsertAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class, IEntity
        {
            _db.AddRange(entities);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            _db.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<TEntity> DeleteAsync<TEntity>(int id)
            where TEntity : class, IEntity
        {
            var e = await _db.FindAsync<TEntity>(id);
            await DeleteAsync(e);
            return e;
        }

        public IQueryable<TEntity> Query<TEntity>()
            where TEntity : class, IEntity
        {
            return _db.Set<TEntity>();
        }

        public IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>> filter)
            where TEntity : class, IEntity
        {
            return _db.Set<TEntity>().Where(filter);
        }

        public IQueryable<TEntity> Query<TEntity>(int id)
            where TEntity : class, IEntity
        {
            return _db.Set<TEntity>().Where(e => e.Id == id);
        }

        public IQueryable<TEntity> Query<TEntity>(IEnumerable<int> ids)
            where TEntity : class, IEntity
        {
            return _db.Set<TEntity>().Where(e => ids.Contains(e.Id));
        }

        public void Add<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            _db.Add(entity);
        }

        public void Remove<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            _db.Remove(entity);
        }

        public Task<int> SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
