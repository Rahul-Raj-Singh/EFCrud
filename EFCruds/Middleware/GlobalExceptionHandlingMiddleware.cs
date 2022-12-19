using Newtonsoft.Json;

namespace EFCruds.Middleware;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        this.logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch(Exception e)
        {
            logger.LogError(e, e.Message);

            var response = new {Title = "Server Error", StatusCode = 500, Details = $"Error: {e.Message}. Refer logs at timestamp {DateTime.Now}"};
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(response);
            
        }
    }
}
