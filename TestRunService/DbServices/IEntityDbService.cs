using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SG.TestRunService.DbServices
{
    public interface IEntityDbService<TEntity>
    {
        Task<IEnumerable<T>> GetAllAsync<T>(Expression<Func<TEntity, T>> projection);
        Task<IEnumerable<T>> GetFilteredAsync<T>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, T>> projection);
        Task InsertAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<TEntity> DeleteAsync(int id);
    }
}
