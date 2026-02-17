using Microsoft.AspNetCore.Mvc;
using webshop_owp.Data.Services;

namespace webshop_owp.Data.ViewComponents
{
    public class CategoryDropDown : ViewComponent
    {
        private readonly ICategoriesService _service;
        public CategoryDropDown(ICategoriesService service)
        {
            _service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _service.GetAllAsync();
            return View(categories);
        }
    }
}
