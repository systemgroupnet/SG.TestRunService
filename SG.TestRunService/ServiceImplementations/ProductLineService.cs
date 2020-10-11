using Microsoft.EntityFrameworkCore;
using SG.TestRunService.Common.Models;
using SG.TestRunService.Data;
using SG.TestRunService.Data.Services;
using SG.TestRunService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations
{
    public class ProductLineService : IProductLineService
    {
        private readonly IBaseDbService _dbService;

        public ProductLineService(IBaseDbService dbService)
        {
            _dbService = dbService;
        }

        public async Task<(IReadOnlyList<Data.ProductLine>, ServiceError)> GetAllProductLines()
        {
            return (await _dbService.GetAllAsync<Data.ProductLine>(), ServiceError.NoError);
        }

        public Task<(Data.ProductLine, ServiceError)> GetProductLineAsync(ProductLineIdOrKey productLine)
        {
            return GetProductLineInternalAsync(productLine, p => p);
        }

        public async Task<(T, ServiceError)> GetProductLineInternalAsync<T>(
            ProductLineIdOrKey productLine,
            Expression<Func<Data.ProductLine, T>> projection)
        {
            Expression<Func<Data.ProductLine, bool>> predicate;
            if (productLine.Id.HasValue)
                predicate = p => p.Id == productLine.Id.Value;
            else if (productLine.Key != null)
                predicate = p => p.Key == productLine.Key;
            else
                return (default, ServiceError.BadRequest("No ProductLine information is provided."));

            var result = await _dbService.Query(predicate).Select(projection).FirstOrDefaultAsync();
            if (result == null)
                return (default, ServiceError.NotFound("ProductLine not found."));
            return (result, ServiceError.NoError);
        }

        public async Task<(int, ServiceError)> GetProductLineIdAsync(ProductLineIdOrKey productLine)
        {
            if (productLine.Id.HasValue)
                return (productLine.Id.Value, ServiceError.NoError);
            return await GetProductLineInternalAsync(productLine, p => p.Id);
        }

        public async Task<(Data.ProductLine, ServiceError)> GetOrInsertProductLineAsync(Common.Models.ProductLine productLineRequest)
        {
            var (productLine, error) = await GetProductLineAsync(productLineRequest);
            return error.Category switch
            {
                ServiceErrorCategory.NoError => (productLine, error),
                ServiceErrorCategory.NotFound => await InsertProductLineAsync(productLineRequest),
                _ => (null, error)
            };
        }

        private async Task<(Data.ProductLine, ServiceError)> InsertProductLineAsync(Common.Models.ProductLine productLineRequest)
        {
            if (string.IsNullOrEmpty(productLineRequest.Key))
            {
                return (null, ServiceError.BadRequest("The Key is required to insert a new ProductLine."));
            }
            var productLine = new Data.ProductLine()
            {
                Key = productLineRequest.Key,
                AzureProductBuildDefinitionId = productLineRequest.AzureProductBuildDefinitionId
            };
            await _dbService.InsertAsync(productLine);

            return (productLine, ServiceError.NoError);
        }
    }
}
