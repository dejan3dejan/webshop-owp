using webshop_owp.Data.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webshop_owp.Models
{
    /// <summary>
    /// Represents a catalog item available for purchase in the webshop.
    /// </summary>
    public class Product : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        [Display(Name = "Inventory Stock")]
        public int StockAmount { get; set; } = 10;

        [Display(Name = "Discount %")]
        public int DiscountPercentage { get; set; } = 0;

        /// <summary>Comma-separated feature tags used for content-based ML filtering. Example: "wireless,bluetooth,over-ear"</summary>
        [Display(Name = "Tags (comma-separated)")]
        public string? Tags { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}