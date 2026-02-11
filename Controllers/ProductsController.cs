using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webshop_owp.Data;

namespace webshop_owp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var allProducts = await _context.Products.Include(n => n.Category).ToListAsync();
            return View(allProducts);
        }
    }
}