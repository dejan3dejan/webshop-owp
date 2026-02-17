using Microsoft.EntityFrameworkCore;
using webshop_owp.Data.Base;
using webshop_owp.Data.ViewModels;
using webshop_owp.Models;

namespace webshop_owp.Data.Services
{
    public class ProductsService : EntityBaseRepository<Product>, IProductsService
    {
        public ProductsService(AppDbContext context) : base(context) { }

        public async Task<NewProductDropdownsVM> GetNewProductDropdownsValues()
        {
            var response = new NewProductDropdownsVM()
            {
                Categories = await _context.Categories.OrderBy(n => n.Name).ToListAsync()
            };
            return response;
        }

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
        }

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
            }
        }
    }
}
