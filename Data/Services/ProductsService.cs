using Microsoft.EntityFrameworkCore;
using webshop_owp.Data.Base;
using webshop_owp.Data.ViewModels;
using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    /// <summary>
    /// Implements operations for managing the product catalog, including search and pagination functionality.
    /// </summary>
    public class ProductsService : EntityBaseRepository<Product>, IProductsService
    {
        private readonly ILogger<ProductsService> _logger;

        public ProductsService(AppDbContext context, ILogger<ProductsService> logger) : base(context) 
        { 
            _logger = logger;
        }

        /// <summary>
        /// Retrieves reference data needed to populate dropdowns when creating or editing a product.
        /// </summary>
        /// <returns>A view model containing available categories and other reference lists.</returns>
        public async Task<NewProductDropdownsVM> GetNewProductDropdownsValues()
        {
            var response = new NewProductDropdownsVM()
            {
                Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync()
            };
            return response;
        }

        /// <summary>
        /// Processes and persists a newly created product from its corresponding view model representation.
        /// </summary>
        /// <param name="data">The view model holding the new product values.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        public async Task AddNewProductAsync(NewProductVM data)
        {
            var newProduct = new Product()
            {
                Name = data.Name,
                Description = data.Description,
                Price = data.Price,
                ImageUrl = data.ImageUrl,
                CategoryId = data.CategoryId,
                StockAmount = data.StockAmount
            };
            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added new product: {ProductName}", newProduct.Name);
        }

        /// <summary>
        /// Applies modify operations to an existing product using its view model representation.
        /// </summary>
        /// <param name="data">The view model holding the updated product values.</param>
        /// <returns>A task representing the asynchronous update and save operation.</returns>
        public async Task UpdateProductAsync(NewProductVM data)
        {
            var dbProduct = await _context.Products.FirstOrDefaultAsync(n => n.Id == data.Id);

            if (dbProduct != null)
            {
                dbProduct.Name = data.Name;
                dbProduct.Description = data.Description;
                dbProduct.Price = data.Price;
                dbProduct.ImageUrl = data.ImageUrl;
                dbProduct.CategoryId = data.CategoryId;
                dbProduct.StockAmount = data.StockAmount;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated product with ID: {ProductId}", data.Id);
            }
        }
        /// <summary>
        /// Retrieves a specialized paginated subset of the product catalog, optionally filtered by a specific category.
        /// </summary>
        /// <param name="categoryId">An optional category ID to filter by.</param>
        /// <param name="pageNumber">The 1-based page index to retrieve.</param>
        /// <param name="pageSize">The number of items to display per page.</param>
        /// <returns>A paginated list of catalog products matching the criteria.</returns>
        public async Task<PaginatedList<Product>> GetPaginatedAsync(int? categoryId, int pageNumber, int pageSize)
        {
            var productsQuery = _context.Products.Include(n => n.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            return await PaginatedList<Product>.CreateAsync(productsQuery.AsNoTracking(), pageNumber, pageSize);
        }

        /// <summary>
        /// Performs a text-based search across the product catalog and returns a paginated subset.
        /// </summary>
        /// <param name="searchString">The text to locate within the product names and descriptions.</param>
        /// <param name="pageNumber">The 1-based page index to retrieve.</param>
        /// <param name="pageSize">The number of items to display per page.</param>
        /// <returns>A paginated list of catalog products resembling the search parameter.</returns>
        public async Task<PaginatedList<Product>> SearchAsync(string searchString, int pageNumber, int pageSize)
        {
            var productsQuery = _context.Products.Include(n => n.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(n => n.Name.Contains(searchString) || n.Description.Contains(searchString));
            }

            return await PaginatedList<Product>.CreateAsync(productsQuery.AsNoTracking(), pageNumber, pageSize);
        }
    }
}
