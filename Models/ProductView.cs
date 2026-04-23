using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webshop_owp.Models
{
    public class ProductView
    {
        [Key]
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        
        public string? UserId { get; set; }     // null = anonymous visitor
        public string SessionId { get; set; }   // always populated from session cookie
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
        public int DurationSeconds { get; set; } = 0;
    }
}
