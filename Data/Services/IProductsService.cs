using webshop_owp.Data.Base;
using webshop_owp.Data.ViewModels;
using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    public interface IProductsService : IEntityBaseRepository<Product>
    {
        Task<NewProductDropdownsVM> GetNewProductDropdownsValues();
        Task AddNewProductAsync(NewProductVM data);
        Task UpdateProductAsync(NewProductVM data);
    }
}
