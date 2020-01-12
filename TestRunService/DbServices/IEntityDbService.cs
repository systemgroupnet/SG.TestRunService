using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SG.TestRunService.DbServices
{
    public interface IEntityDbService<TEntity>
    {
        Task<IReadOnlyList<T>> GetAllAsync<T>(Expression<Func<TEntity, T>> projection);
        Task<IReadOnlyList<T>> GetAllAsync<T>(IMapper mapper);
        Task<IReadOnlyList<T>> GetFilteredAsync<T>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, T>> projection);
        Task<IReadOnlyList<T>> GetFilteredAsync<T>(Expression<Func<TEntity, bool>> filter, IMapper mapper);
        Task<T> GetFirstOrDefaultAsync<T>(Expression<Func<TEntity, bool>> filter, IMapper mapper);
        Task InsertAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<TEntity> DeleteAsync(int id);
    }
}
