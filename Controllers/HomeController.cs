using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webshop_owp.Data.Services;
using webshop_owp.Models;

namespace webshop_owp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductsService _productsService;

    public HomeController(ILogger<HomeController> logger, IProductsService productsService)
    {
        _logger = logger;
        _productsService = productsService;
    }

    public async Task<IActionResult> Index()
    {
        var allProducts = await _productsService.GetAllAsync(n => n.Category);
        var featuredProducts = allProducts.OrderByDescending(n => n.Id).Take(3).ToList();
        return View(featuredProducts);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
