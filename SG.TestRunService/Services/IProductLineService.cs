using SG.TestRunService.Common.Models;
using System.Threading.Tasks;

namespace SG.TestRunService.Services
{
    public interface IProductLineService
    {
        Task<(Data.ProductLine, ServiceError)> GetProductLineAsync(ProductLineIdOrKey productLine);
        Task<(int, ServiceError)> GetProductLineIdAsync(ProductLineIdOrKey productLine);
        Task<(Data.ProductLine, ServiceError)> GetOrInsertProductLineAsync(Common.Models.ProductLine productLineRequest);
    }
}
