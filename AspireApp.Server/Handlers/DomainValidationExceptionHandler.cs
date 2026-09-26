using App.Model.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Json;

namespace AspireApp.Server.Handlers
{
    public class DomainValidationExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<DomainValidationExceptionHandler> logger;
        public DomainValidationExceptionHandler(ILogger<DomainValidationExceptionHandler> logger)
        {
            this.logger = logger;
        }
        public ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var exceptionMessage = exception.Message;
            logger.LogError(
                "Error Message: {exceptionMessage}, Time of occurrence {time}",
                exceptionMessage, DateTime.UtcNow);
            if (exception is DomainValidationException)
            {
                httpContext.Response.StatusCode = 403;
                httpContext.Response.WriteAsJsonAsync(JsonSerializer.Serialize(((DomainValidationException)exception).Validators), cancellationToken);
                return ValueTask.FromResult(true);
            }
            return ValueTask.FromResult(false);
        }
    }
}
