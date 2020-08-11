using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using SG.TestRunService.Data.Services;
using SG.TestRunService.Services;
using System;
using System.Threading.Tasks;

namespace SG.TestRunService.ServiceImplementations
{
    public class PollyRetryDbErrorFacility : IRetryFacility
    {
        private const int MaxRetries = 3;
        private const string BaseDbServiceContextKey = "BaseDbService";
        public static readonly IAsyncPolicy RetryPolicy;

        private readonly IBaseDbService _dbService;

        static PollyRetryDbErrorFacility()
        {
            RetryPolicy = Policy
                .Handle<SqlException>()
                .Or<DbUpdateException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryCount => TimeSpan.FromSeconds(5),
                    onRetry: OnRetry);
        }

        public PollyRetryDbErrorFacility(IBaseDbService dbService)
        {
            _dbService = dbService;
        }

        private static void OnRetry(Exception ex, TimeSpan sleep, int retryCount, Context context)
        {
            Serilog.Log.Warning(ex,
                "An exception is thrown while processing the request. Operation: {Operation}, Attempt {Attempt}/{MaxRetries}",
                context.OperationKey, retryCount, MaxRetries);
            var dbService = (IBaseDbService)context[BaseDbServiceContextKey];
            dbService.ResetDbContext();
        }

        public Task RetryAsync(string operationName, Func<Task> action)
        {
            var context = new Context(operationName);
            context[BaseDbServiceContextKey] = _dbService;
            return RetryPolicy.ExecuteAsync(ctx => action(), context);
        }

        public Task<TResult> RetryAsync<TResult>(string operationName, Func<Task<TResult>> action)
        {
            var context = new Context(operationName);
            context[BaseDbServiceContextKey] = _dbService;
            return RetryPolicy.ExecuteAsync(ctx => action(), context);
        }
    }
}
