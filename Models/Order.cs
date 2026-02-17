using webshop_owp.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace webshop_owp.Models
{
    public class Order : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }
        
        public string? UserId { get; set; } // Nullable za Guest checkout

        public string FullName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        public double TotalAmount { get; set; }
        public string? CouponCode { get; set; }
        public double DiscountAmount { get; set; }

        // Relacija: Jedna narudžbina ima više stavki
        public List<OrderItem> OrderItems { get; set; }
    }
}