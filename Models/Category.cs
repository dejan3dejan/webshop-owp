using System.ComponentModel.DataAnnotations;

using webshop_owp.Data.Base;

namespace webshop_owp.Models
{
    public class Category : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        public List<Product>? Products { get; set; }
    }
}