using webshop_owp.Data.Base;
using webshop_owp.Data.ViewModels;
using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    /// <summary>
    /// Provides operations for managing the product catalog, including search and pagination features.
    /// </summary>
    public interface IProductsService : IEntityBaseRepository<Product>
    {
        /// <summary>
        /// Retrieves reference data needed to populate dropdowns when creating or editing a product.
        /// </summary>
        /// <returns>A view model containing available categories and other reference lists.</returns>
        Task<NewProductDropdownsVM> GetNewProductDropdownsValues();
        /// <summary>
        /// Processes and persists a newly created product from its corresponding view model representation.
        /// </summary>
        /// <param name="data">The view model holding the new product values.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task AddNewProductAsync(NewProductVM data);
        /// <summary>
        /// Applies modify operations to an existing product using its view model representation.
        /// </summary>
        /// <param name="data">The view model holding the updated product values.</param>
        /// <returns>A task representing the asynchronous update and save operation.</returns>
        Task UpdateProductAsync(NewProductVM data);
        /// <summary>
        /// Retrieves a specialized paginated subset of the product catalog, optionally filtered by a specific category.
        /// </summary>
        /// <param name="categoryId">An optional category ID to filter by.</param>
        /// <param name="pageNumber">The 1-based page index to retrieve.</param>
        /// <param name="pageSize">The number of items to display per page.</param>
        /// <returns>A paginated list of catalog products matching the criteria.</returns>
        Task<PaginatedList<Product>> GetPaginatedAsync(int? categoryId, int pageNumber, int pageSize);
        /// <summary>
        /// Performs a text-based search across the product catalog and returns a paginated subset.
        /// </summary>
        /// <param name="searchString">The text to locate within the product names and descriptions.</param>
        /// <param name="pageNumber">The 1-based page index to retrieve.</param>
        /// <param name="pageSize">The number of items to display per page.</param>
        /// <returns>A paginated list of catalog products resembling the search parameter.</returns>
        Task<PaginatedList<Product>> SearchAsync(string searchString, int pageNumber, int pageSize);
    }
}
