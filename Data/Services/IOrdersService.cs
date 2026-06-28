using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    public interface IOrdersService
    {
        Task StoreOrderAsync(List<ShoppingCartItem> items, string? userId, string userEmailAddress, string fullName, string address, string city, string? couponCode = null);
        Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string? userId, string? userRole);
        Task<Coupon?> GetCouponByCodeAsync(string code);
    }
}
