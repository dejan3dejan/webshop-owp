using Microsoft.EntityFrameworkCore;
using webshop_owp.Models;
using webshop_owp.Data.Static;

namespace webshop_owp.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;
        public OrdersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string? userId, string? userRole)
        {
            var query = _context.Orders
                .Include(n => n.OrderItems).ThenInclude(n => n.Product)
                .AsQueryable();

            if (userRole != UserRoles.Admin)
            {
                query = query.Where(n => n.UserId == userId);
            }

            return await query.ToListAsync();
        }

        public async Task StoreOrderAsync(List<ShoppingCartItem> items, string? userId, string userEmailAddress, string fullName, string address, string city, string? couponCode = null)
        {
            decimal total = 0;
            foreach (var item in items)
            {
                var price = item.Product.Price;
                if (item.Product.DiscountPercentage > 0)
                {
                    price = price * (1 - item.Product.DiscountPercentage / 100m);
                }
                total += price * item.Amount;
            }

            decimal discountAmount = 0;
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await GetCouponByCodeAsync(couponCode);
                if (coupon != null)
                {
                    discountAmount = total * ((decimal)coupon.DiscountPercentage / 100m);
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
                    price = price * (1 - item.Product.DiscountPercentage / 100m);
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
        }

        public async Task<Coupon?> GetCouponByCodeAsync(string code)
        {
            return await _context.Coupons.FirstOrDefaultAsync(n => n.Code == code && n.IsActive);
        }
    }
}
