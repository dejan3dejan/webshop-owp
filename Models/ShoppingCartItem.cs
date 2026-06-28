using webshop_owp.Data.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webshop_owp.Models
{
    public class ShoppingCartItem
    {
        [Key]
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public int Amount { get; set; }
        
        public string ShoppingCartId { get; set; } = string.Empty;  
    }
}