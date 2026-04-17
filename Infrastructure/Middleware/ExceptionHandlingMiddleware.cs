using System.Text.Json;

namespace webshop_owp.Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during the request.");

                if (IsAjaxRequest(context.Request))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 500;
                    
                    var response = new { error = "An unexpected error occurred." };
                    var json = JsonSerializer.Serialize(response);
                    await context.Response.WriteAsync(json);
                }
                else
                {
                    context.Response.Redirect("/Home/Error");
                }
            }
        }

        private bool IsAjaxRequest(HttpRequest request)
        {
            return string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        }
    }
}
