using System.ComponentModel.DataAnnotations;
using webshop_owp.Data.Base;

namespace webshop_owp.Models
{
    public class Coupon : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Coupon Code")]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 100)]
        [Display(Name = "Discount Percentage")]
        public double DiscountPercentage { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
