using Gateway.Response;
using System.Net;
using System.Text.Json;

namespace Gateway.Middleware
{
    public class ResultMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResultMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ResultMiddleware(RequestDelegate next, ILogger<ResultMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Capture the response body to check the status code later
                var originalResponseBody = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                // Process the response based on the status code
                context.Response.Body = originalResponseBody;
                await HandleResponseAsync(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleResponseAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            ResultResponse response;

            switch (context.Response.StatusCode)
            {
                case (int)HttpStatusCode.NotFound:
                    response = new ResultResponse(false, null, new[] { new Error("Not Found", "The requested resource was not found.") });
                    break;

                case (int)HttpStatusCode.Unauthorized:
                    response = new ResultResponse(false, null, new[] { new Error("Unauthorized", "You are not authorized to access this resource.") });
                    break;

                case (int)HttpStatusCode.Forbidden:
                    response = new ResultResponse(false, null, new[] { new Error("Forbidden", "You are not allowed to access this resource.") });
                    break;

                case (int)HttpStatusCode.BadRequest:
                    response = new ResultResponse(false, null, new[] { new Error("Bad Request", "The request is invalid.") });
                    break;

                case (int)HttpStatusCode.OK:
                case (int)HttpStatusCode.Created:
                    response = new ResultResponse(true, new Data("I am content"), null);
                    break;

                default:
                    // If no custom handling is required, write the response as it is
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    await context.Response.Body.CopyToAsync(context.Response.Body);
                    return;
            }

            // Serialize and write the response
            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment()
                ? new ResultResponse(false, null, new[] { new Error(ex.Message, ex.StackTrace ?? string.Empty) })
                : new ResultResponse(false, null, new[] { new Error("Internal Server Error", string.Empty) });

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
