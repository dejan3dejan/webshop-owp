using webshop_owp.Models;

namespace webshop_owp.Data.ViewModels
{
    public class NewProductDropdownsVM
    {
        public NewProductDropdownsVM()
        {
            Categories = new List<Category>();
        }

        public List<Category> Categories { get; set; }
    }
}