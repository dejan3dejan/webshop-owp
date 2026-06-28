using System.ComponentModel.DataAnnotations;

namespace webshop_owp.Data.ViewModels
{
    public class NewProductVM
    {
        public int Id { get; set; }

        [Display(Name = "Product name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Display(Name = "Product description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Price in $")]
        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        [Display(Name = "Image URL")]
        [Required(ErrorMessage = "Image URL is required")]
        public string ImageUrl { get; set; }

        [Display(Name = "Stock Quantity")]
        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, 1000, ErrorMessage = "Stock must be between 0 and 1000")]
        public int StockAmount { get; set; }

        [Display(Name = "Select a category")]
        [Required(ErrorMessage = "Product category is required")]
        public int CategoryId { get; set; }
    }
}