using Microsoft.EntityFrameworkCore;
using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    /// <summary>
    /// Implements operations for placing customer orders, calculating discounts, and retrieving order history.
    /// </summary>
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrdersService> _logger;

        public OrdersService(AppDbContext context, ILogger<OrdersService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the order history for a specific user, or all system orders if the user is an administrator. Includes nested items and product details.
        /// </summary>
        /// <param name="userId">The unique identifier of the requesting user.</param>
        /// <param name="userRole">The role of the requesting user to determine data access scope.</param>
        /// <returns>A collection of matching orders.</returns>
        public async Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole)
        {
            var orders = await _context.Orders.Include(n => n.OrderItems).ThenInclude(n => n.Product).ToListAsync();

            if (userRole != "Admin")
            {
                orders = orders.Where(n => n.UserId == userId).ToList();
            }

            return orders;
        }

        /// <summary>
        /// Transactionally processes the checkout out by subtracting stock levels, calculating discounts, computing the final totals, and saving the master order record.
        /// </summary>
        /// <param name="items">The items currently residing in the user's shopping cart.</param>
        /// <param name="userId">The identifier of the user placing the order (nullable for guests).</param>
        /// <param name="userEmailAddress">The billing and contact email address.</param>
        /// <param name="fullName">The full name of the recipient.</param>
        /// <param name="address">The delivery street address.</param>
        /// <param name="city">The delivery city.</param>
        /// <param name="couponCode">An optional promotional code to apply discounts.</param>
        /// <returns>A task that represents the asynchronous database commit operation.</returns>
        public async Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress, string fullName, string address, string city, string? couponCode = null)
        {
            double total = 0;
            foreach (var item in items)
            {
                var price = item.Product.Price;
                if (item.Product.DiscountPercentage > 0)
                {
                    price = price * (1 - item.Product.DiscountPercentage / 100.0);
                }
                total += price * item.Amount;
            }

            double discountAmount = 0;
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await GetCouponByCodeAsync(couponCode);
                if (coupon != null)
                {
                    discountAmount = total * (coupon.DiscountPercentage / 100.0);
                }
            }

            var order = new Order()
            {
                UserId = userId,
                Email = userEmailAddress,
                FullName = fullName,
                Address = address,
                City = city,
                TotalAmount = total - discountAmount,
                CouponCode = couponCode,
                DiscountAmount = discountAmount
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var item in items)
            {
                var product = await _context.Products.FirstOrDefaultAsync(n => n.Id == item.Product.Id);
                if (product != null)
                {
                    product.StockAmount -= item.Amount;
                }

                var price = item.Product.Price;
                if (item.Product.DiscountPercentage > 0)
                {
                    price = price * (1 - item.Product.DiscountPercentage / 100.0);
                }

                var orderItem = new OrderItem()
                {
                    Amount = item.Amount,
                    ProductId = item.Product.Id,
                    OrderId = order.Id,
                    Price = price
                };
                await _context.OrderItems.AddAsync(orderItem);
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully stored order with ID: {OrderId} for User: {UserId}", order.Id, userId);
        }

        /// <summary>
        /// Validates and retrieves an active coupon definition from the database by its code.
        /// </summary>
        /// <param name="code">The promotional string submitted by the user.</param>
        /// <returns>The coupon model if valid and active, otherwise null.</returns>
        public async Task<Coupon> GetCouponByCodeAsync(string code)
        {
            return await _context.Coupons.FirstOrDefaultAsync(n => n.Code == code && n.IsActive);
        }
    }
}
