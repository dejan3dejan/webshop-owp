using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webshop_owp.Data.Services;
using webshop_owp.Data.Static;
using webshop_owp.Data.ViewModels;
using webshop_owp.Models;
using webshop_owp.Data.Base;

namespace webshop_owp.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class ProductsController : Controller
    {
        private readonly IProductsService _service;
        public ProductsController(IProductsService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoryId, int pageNumber = 1)
        {
            int pageSize = 6;
            
            if (categoryId.HasValue)
            {
                ViewBag.CurrentCategoryId = categoryId;
            }

            var paginatedProducts = await _service.GetPaginatedAsync(categoryId, pageNumber, pageSize);
            return View(paginatedProducts);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Filter(string searchString, int pageNumber = 1)
        {
            int pageSize = 6;
            
            if (!string.IsNullOrEmpty(searchString))
            {
                ViewBag.CurrentFilter = searchString;
            }

            var paginatedProducts = await _service.SearchAsync(searchString, pageNumber, pageSize);
            return View("Index", paginatedProducts);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var productDetails = await _service.GetByIdAsync(id, n => n.Category);

            if (productDetails == null) return View("NotFound");
            return View(productDetails);
        }

        public async Task<IActionResult> Create()
        {
            var productDropdownsData = await _service.GetNewProductDropdownsValues();
            ViewBag.Categories = new SelectList(productDropdownsData.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewProductVM product)
        {
            if (!ModelState.IsValid)
            {
                var productDropdownsData = await _service.GetNewProductDropdownsValues();
                ViewBag.Categories = new SelectList(productDropdownsData.Categories, "Id", "Name");
                return View(product);
            }

            await _service.AddNewProductAsync(product);
            TempData["Success"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var productDetails = await _service.GetByIdAsync(id);
            if (productDetails == null) return View("NotFound");

            var response = new NewProductVM()
            {
                Id = productDetails.Id,
                Name = productDetails.Name,
                Description = productDetails.Description,
                Price = productDetails.Price,
                ImageUrl = productDetails.ImageUrl,
                CategoryId = productDetails.CategoryId,
            };

            var productDropdownsData = await _service.GetNewProductDropdownsValues();
            ViewBag.Categories = new SelectList(productDropdownsData.Categories, "Id", "Name");

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewProductVM product)
        {
            if (id != product.Id) return View("NotFound");

            if (!ModelState.IsValid)
            {
                var productDropdownsData = await _service.GetNewProductDropdownsValues();
                ViewBag.Categories = new SelectList(productDropdownsData.Categories, "Id", "Name");
                return View(product);
            }

            await _service.UpdateProductAsync(product);
            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var productDetails = await _service.GetByIdAsync(id, n => n.Category);
            if (productDetails == null) return View("NotFound");
            return View(productDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productDetails = await _service.GetByIdAsync(id);
            if (productDetails == null) return View("NotFound");

            await _service.DeleteAsync(id);
            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
