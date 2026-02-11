using System.ComponentModel.DataAnnotations;

namespace webshop_owp.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string Email { get; set; }
        
        public string UserId { get; set; }

        // Relacija: Jedna narudžbina ima više stavki
        public List<OrderItem> OrderItems { get; set; }
    }
}