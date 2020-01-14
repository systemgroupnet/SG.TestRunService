using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SG.TestRunService.Data;
using SG.TestRunService.DbServices;

namespace SG.TestRunService.DbServices.Implementations
{
    public class EntityDbServiceBase<TEntity> : IEntityDbService<TEntity>
        where TEntity : class, IEntity
    {
        protected TSDbContext Db { get; private set; }

        public EntityDbServiceBase(TSDbContext db)
        {
            Db = db;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync<T>(Expression<Func<TEntity, T>> projection)
        {
            return await Db.Set<TEntity>().Select(projection).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync<T>(Func<IQueryable<TEntity>, IQueryable<T>> projector)
        {
            return await projector(Db.Set<TEntity>()).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetFilteredAsync<T>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, T>> projection)
        {
            return await Db.Set<TEntity>().Where(filter).Select(projection).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetFilteredAsync<T>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<T>> projector)
        {
            return await projector(Db.Set<TEntity>().Where(filter)).ToListAsync();
        }

        public async Task<T> GetFirstOrDefaultAsync<T>(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IQueryable<T>> projector)
        {
            return await projector(Db.Set<TEntity>().Where(filter)).FirstOrDefaultAsync();
        }

        public Task<T> GetById<T>(int id, Func<IQueryable<TEntity>, IQueryable<T>> projector)
        {
            return GetFirstOrDefaultAsync(e => e.Id == id, projector);
        }

        public async Task InsertAsync(TEntity entity)
        {
            Db.Add(entity);
            await Db.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            Db.Entry(entity).State = EntityState.Modified;
            await Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            Db.Remove(entity);
            await Db.SaveChangesAsync();
        }

        public async Task<TEntity> DeleteAsync(int id)
        {
            var e = await Db.FindAsync<TEntity>(id);
            await DeleteAsync(e);
            return e;
        }
    }
}

