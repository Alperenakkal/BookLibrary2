
using System.Net;
using System.Text.Json;

namespace BookLibary.Api.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;
        public ErrorHandlerMiddleware(RequestDelegate next,ILogger<ErrorHandlerMiddleware> logger)
        {
             _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case BadRequestException e:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case NotFoundException e:
                        response.StatusCode= (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        response.StatusCode=(int)HttpStatusCode.InternalServerError;
                        break;
                }
                _logger.LogError(error.Message);
                context.Response.StatusCode=response.StatusCode;
                var resultMessage = JsonSerializer.Serialize(new {message=error?.Message});
                await context.Response.WriteAsync(resultMessage);

                
            }
        }
    }
}
