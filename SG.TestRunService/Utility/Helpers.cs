using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Utility
{
    public static class Helpers
    {
        private const int MaxRetries = 3;
        public static readonly IAsyncPolicy RetryPolicy;

        static Helpers()
        {
            RetryPolicy = Policy
                .Handle<SqlException>()
                .Or<DbUpdateException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryCount => TimeSpan.FromSeconds(5),
                    onRetry: OnRetry
                    );
        }

        private static void OnRetry(Exception ex, TimeSpan sleep, int retryCount, Context context)
        {
            Serilog.Log.Warning(ex,
                "An exception is thrown while processing the request. Operation: {Operation}, Attempt {Attempt}/{MaxRetries}",
                context.OperationKey, retryCount, MaxRetries);
        }

        public static Task RetryAsync(string operationName, Func<Task> action)
        {
            return RetryPolicy.ExecuteAsync(ctx => action(), new Context(operationName));
        }

        public static Task<TResult> RetryAsync<TResult>(string operationName, Func<Task<TResult>> action)
        {
            return RetryPolicy.ExecuteAsync(ctx => action(), new Context(operationName));
        }
    }
}
