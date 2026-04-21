using webshop_owp.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace webshop_owp.Models
{
    /// <summary>
    /// Represents a customer's placed order, containing billing and shipping information alongside the purchase totals.
    /// </summary>
    public class Order : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; }
        
        public string? UserId { get; set; } // Nullable za Guest checkout

        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        
        [Display(Name = "Address")]
        public string Address { get; set; }
        
        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Total Amount")]
        public double TotalAmount { get; set; }
        
        [Display(Name = "Coupon Code")]
        public string? CouponCode { get; set; }
        
        [Display(Name = "Discount Amount")]
        public double DiscountAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Completed;

        // Relacija: Jedna narudžbina ima više stavki
        public List<OrderItem> OrderItems { get; set; }
    }
}