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
        public double Price { get; set; }

        [Display(Name = "Product image URL")]
        [Required(ErrorMessage = "Image URL is required")]
        public string ImageUrl { get; set; }

        [Display(Name = "Select a category")]
        [Required(ErrorMessage = "Product category is required")]
        public int CategoryId { get; set; }
    }
}