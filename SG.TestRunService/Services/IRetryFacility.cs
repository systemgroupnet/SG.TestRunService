using System;
using System.Threading.Tasks;

namespace SG.TestRunService.Services
{
    public interface IRetryFacility
    {
        Task RetryAsync(string operationName, Func<Task> action);
        Task<TResult> RetryAsync<TResult>(string operationName, Func<Task<TResult>> action);
    }
}
