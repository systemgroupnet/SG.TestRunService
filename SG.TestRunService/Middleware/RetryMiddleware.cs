using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Middleware
{
    public class RetryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RetryMiddleware> _logger;
        private const int MaxRetries = 3;
        private readonly TimeSpan RetryWait = TimeSpan.FromSeconds(5);
        private readonly Func<Exception, bool> ExceptionFilter =
            ex =>
                ex is SqlException ||
                ex is DbUpdateConcurrencyException;

        public RetryMiddleware(RequestDelegate next, ILogger<RetryMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            List<Exception> exceptions = null;
            int attempt = 0;
            while (true)
            {
                try
                {
                    if (attempt > 0)
                        await Task.Delay(RetryWait);
                    await _next(context);
                    return;
                }
                catch (Exception ex) when (ExceptionFilter(ex))
                {
                    _logger.LogWarning(ex,
                        "An exception is thrown while processing the request. Attempt {Attempt}/{MaxRetries}",
                        attempt+1, MaxRetries);

                    if(context.Response.HasStarted)
                    {
                        _logger.LogWarning("Cannot retry operation since the response has already been started");
                        throw;
                    }

                    AddExceptionIfNotAlreadyThere(ex, ref exceptions);

                    ++attempt;
                    if (attempt == MaxRetries)
                    {
                        if (exceptions.Count == 1) // same exception thrown multiple times
                            throw;
                        else
                            throw new AggregateException(exceptions);
                    }
                }
            }
        }

        private static void AddExceptionIfNotAlreadyThere(Exception ex, ref List<Exception> exceptions)
        {
            if (exceptions == null)
            {
                exceptions = new List<Exception>() { ex };
            }
            else
            {
                bool found = false;
                foreach (var oldEx in exceptions)
                {
                    if (oldEx.GetType() == ex.GetType() && oldEx.Message == ex.Message)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    exceptions.Add(ex);
            }
        }
    }
}
