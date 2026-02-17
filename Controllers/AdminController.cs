using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webshop_owp.Data;
using webshop_owp.Data.Static;
using webshop_owp.Models;

namespace webshop_owp.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ProductsCount = await _context.Products.CountAsync();
            ViewBag.CategoriesCount = await _context.Categories.CountAsync();
            ViewBag.OrdersCount = await _context.Orders.CountAsync();
            ViewBag.UsersCount = await _context.Users.CountAsync();

            return View();
        }

        public async Task<IActionResult> Inventory()
        {
            var products = await _context.Products.Include(n => n.Category).OrderBy(n => n.Name).ToListAsync();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStock(int id, int amount)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.StockAmount += amount;
                if (product.StockAmount < 0) product.StockAmount = 0;
                await _context.SaveChangesAsync();
                return Json(new { success = true, newStock = product.StockAmount });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDiscount(int id, int discount)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.DiscountPercentage = discount;
                if (product.DiscountPercentage < 0) product.DiscountPercentage = 0;
                if (product.DiscountPercentage > 100) product.DiscountPercentage = 100;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
