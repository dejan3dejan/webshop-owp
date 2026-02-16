using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webshop_owp.Data.Services;
using webshop_owp.Data.Static;
using webshop_owp.Models;

namespace webshop_owp.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class CategoriesController : Controller
    {
        private readonly ICategoriesService _service;

        public CategoriesController(ICategoriesService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allCategories = await _service.GetAllAsync();
            return View(allCategories);
        }
        
        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            await _service.AddAsync(category);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var categoryDetails = await _service.GetByIdAsync(id);
            if (categoryDetails == null) return View("NotFound");
            return View(categoryDetails);
        }

        //GET: Categories/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var categoryDetails = await _service.GetByIdAsync(id);
            if (categoryDetails == null) return View("NotFound");
            return View(categoryDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
        {
            await _service.UpdateAsync(id, category);
            return RedirectToAction(nameof(Index));
        }

        //GET: Categories/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var categoryDetails = await _service.GetByIdAsync(id);
            if (categoryDetails == null) return View("NotFound");
            return View(categoryDetails);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryDetails = await _service.GetByIdAsync(id);
            if (categoryDetails == null) return View("NotFound");

            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
