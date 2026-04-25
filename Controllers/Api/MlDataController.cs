using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webshop_owp.Data;
using webshop_owp.Models;

namespace webshop_owp.Controllers.Api
{
    [ApiController]
    [Route("api/ml")]
    [Authorize(Roles = "Admin")]
    public class MlDataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MlDataController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("interactions")]
        public async Task<IActionResult> GetInteractions()
        {
            var validUsersList = await _context.Orders
                .Where(o => o.Status == OrderStatus.Completed && o.UserId != null)
                .Select(o => o.UserId)
                .Distinct()
                .ToListAsync();
            
            var validUsers = validUsersList.Where(u => u != null).Cast<string>().ToHashSet();

            var ordersList = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.Status == OrderStatus.Completed && oi.Order.UserId != null)
                .Select(oi => new { oi.Order.UserId, oi.ProductId })
                .ToListAsync();

            var viewsList = await _context.ProductViews
                .Where(pv => pv.UserId != null)
                .Select(pv => new { pv.UserId, pv.ProductId })
                .ToListAsync();

            var orderGroup = ordersList
                .Where(x => x.UserId != null && validUsers.Contains(x.UserId))
                .GroupBy(x => new { UserId = x.UserId!, x.ProductId })
                .ToDictionary(g => g.Key, g => g.Count());

            var viewsGroup = viewsList
                .Where(x => x.UserId != null && validUsers.Contains(x.UserId))
                .GroupBy(x => new { UserId = x.UserId!, x.ProductId })
                .ToDictionary(g => g.Key, g => g.Count());

            var allKeys = orderGroup.Keys.Union(viewsGroup.Keys).Distinct();

            var result = allKeys.Select(k => new
            {
                userId = k.UserId,
                productId = k.ProductId,
                score = (orderGroup.TryGetValue(k, out var oCount) ? oCount : 0) * 3.0 + 
                        (viewsGroup.TryGetValue(k, out var vCount) ? vCount : 0) * 1.0
            });

            return Ok(result);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    category = p.Category != null ? p.Category.Name : "Unknown",
                    price = p.Price,
                    tags = p.Tags
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.Status == OrderStatus.Completed)
                .Select(o => new
                {
                    orderId = o.Id,
                    userId = o.UserId,
                    createdAt = o.CreatedAt,
                    items = o.OrderItems.Select(oi => new { productId = oi.ProductId, quantity = oi.Amount })
                })
                .ToListAsync();

            return Ok(orders);
        }
    }
}
