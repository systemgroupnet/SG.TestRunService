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
        where TEntity : class
    {
        protected TSDbContext Db { get; private set; }

        public EntityDbServiceBase(TSDbContext db)
        {
            Db = db;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(Expression<Func<TEntity, T>> projection)
        {
            return await Db.Set<TEntity>().Select(projection).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetFilteredAsync<T>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, T>> projection)
        {
            return await Db.Set<TEntity>().Where(filter).Select(projection).ToListAsync();
        }

        public Task InsertAsync(TEntity entity)
        {
            Db.Add(entity);
            return Db.SaveChangesAsync();
        }

        public Task UpdateAsync(TEntity entity)
        {
            Db.Entry(entity).State = EntityState.Modified;
            return Db.SaveChangesAsync();
        }

        public Task DeleteAsync(TEntity entity)
        {
            Db.Remove(entity);
            return Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var e = await Db.FindAsync<TEntity>(id);
            await DeleteAsync(e);
        }
    }
}

