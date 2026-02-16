using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using webshop_owp.Data.Services;
using webshop_owp.Data.Static;
using webshop_owp.Data.ViewModels;
using webshop_owp.Models;

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
        public async Task<IActionResult> Index()
        {
            var allProducts = await _service.GetAllAsync(n => n.Category);
            return View(allProducts);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var productDetails = await _service.GetByIdAsync(id, n => n.Category);

            if (productDetails == null) return View("NotFound");
            return View(productDetails);
        }

        // GET: Products/Create
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
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/1
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
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/1
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
            return RedirectToAction(nameof(Index));
        }
    }
}
