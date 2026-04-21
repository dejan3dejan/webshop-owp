using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    /// <summary>
    /// Provides operations for managing customer orders, checkout processes, and coupons.
    /// </summary>
    public interface IOrdersService
    {
        /// <summary>
        /// Processes a completed checkout by storing the purchased items and customer details into a final order record.
        /// </summary>
        /// <param name="items">The items currently in the user's shopping cart.</param>
        /// <param name="userId">The identifier of the user placing the order (nullable for guests).</param>
        /// <param name="userEmailAddress">The billing and contact email address.</param>
        /// <param name="fullName">The full name of the recipient.</param>
        /// <param name="address">The delivery street address.</param>
        /// <param name="city">The delivery city.</param>
        /// <param name="couponCode">An optional promotional code to apply discounts.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task StoreOrderAsync(List<ShoppingCartItem> items, string userId, string userEmailAddress, string fullName, string address, string city, string? couponCode = null);
        /// <summary>
        /// Retrieves the order history for a specific user, or all system orders if the user has administrative privileges.
        /// </summary>
        /// <param name="userId">The unique identifier of the requesting user.</param>
        /// <param name="userRole">The role of the requesting user to determine data access scope.</param>
        /// <returns>A collection of orders accessible to the specified user.</returns>
        Task<List<Order>> GetOrdersByUserIdAndRoleAsync(string userId, string userRole);
        /// <summary>
        /// Retrieves an active coupon definition by its promotional code.
        /// </summary>
        /// <param name="code">The promotional string submitted by the user.</param>
        /// <returns>The applied coupon details if valid and active, otherwise null.</returns>
        Task<Coupon> GetCouponByCodeAsync(string code);
    }
}
