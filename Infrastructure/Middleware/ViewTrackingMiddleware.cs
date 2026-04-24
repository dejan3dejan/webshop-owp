using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using webshop_owp.Data;
using webshop_owp.Models;

namespace webshop_owp.Infrastructure.Middleware
{
    public class ViewTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly Regex _productDetailsRegex = new Regex(@"^/Products/Details/(\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ViewTrackingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var match = _productDetailsRegex.Match(context.Request.Path);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int productId))
            {
                // Get or create SessionId
                var sessionId = context.Session.GetString("tracking_session_id");
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                    context.Session.SetString("tracking_session_id", sessionId);
                }

                // Get UserId if authenticated
                string? userId = null;
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                }

                // Fire and forget view tracking
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var productView = new ProductView
                        {
                            ProductId = productId,
                            UserId = userId,
                            SessionId = sessionId,
                            ViewedAt = DateTime.UtcNow,
                            DurationSeconds = 0
                        };

                        dbContext.ProductViews.Add(productView);
                        await dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        // Ignore tracking errors to avoid disrupting background thread silently
                    }
                });
            }

            await _next(context);
        }
    }
}
