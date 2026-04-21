using webshop_owp.Data.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webshop_owp.Models
{
    /// <summary>
    /// Represents a specific product and its purchased quantity within an order.
    /// </summary>
    public class OrderItem : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Quantity")]
        public int Amount { get; set; }
        
        [Display(Name = "Price")]
        public double Price { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}