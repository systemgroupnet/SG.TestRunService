using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SG.TestRunService.Services
{
    public class ServiceError
    {
        public ServiceError(ServiceErrorCategory category, string message)
        {
            Category = category;
            Message = message;
        }

        public ServiceErrorCategory Category { get; }
        public string Message { get; }

        public static ServiceError NoError { get; } = new ServiceError(ServiceErrorCategory.NoError, null);

        public static ServiceError NotFound(string message)
        {
            return new ServiceError(ServiceErrorCategory.NotFound, message);
        }

        public static ServiceError UnprocessableEntity(string message)
        {
            return new ServiceError(ServiceErrorCategory.UnprocessableEntity, message);
        }

        public static ServiceError BadRequest(string message)
        {
            return new ServiceError(ServiceErrorCategory.BadRequest, message);
        }
    }

    public enum ServiceErrorCategory
    {
        NoError = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        Conflict = 409,
        Gone = 410,
        UnprocessableEntity = 422
    }

    public static class ServiceErrorExtensions
    {
        public static bool IsSuccessful(this ServiceError serviceError)
        {
            return serviceError == null || serviceError.Category == ServiceErrorCategory.NoError;
        }

        public static IActionResult ToActionResult(this ServiceError serviceError)
        {
            if (serviceError.IsSuccessful())
                return new OkResult();
            int statusCode = ToStatusCode(serviceError.Category);
            if (string.IsNullOrEmpty(serviceError.Message))
                return new StatusCodeResult(statusCode);
            return new ObjectResult(serviceError.Message) { StatusCode = statusCode };
        }

        public static int ToStatusCode(this ServiceErrorCategory? errorCategory)
        {
            return errorCategory switch
            {
                null => StatusCodes.Status200OK,
                ServiceErrorCategory.NoError => StatusCodes.Status200OK,
                ServiceErrorCategory.BadRequest => StatusCodes.Status400BadRequest,
                ServiceErrorCategory.Unauthorized => StatusCodes.Status401Unauthorized,
                ServiceErrorCategory.NotFound => StatusCodes.Status404NotFound,
                ServiceErrorCategory.Conflict => StatusCodes.Status409Conflict,
                ServiceErrorCategory.Gone => StatusCodes.Status410Gone,
                ServiceErrorCategory.UnprocessableEntity => StatusCodes.Status422UnprocessableEntity,
                _ => throw new NotImplementedException()
            };
        }
    }
}
