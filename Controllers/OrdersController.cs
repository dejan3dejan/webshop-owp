using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using webshop_owp.Data.Cart;
using webshop_owp.Data.Services;
using webshop_owp.Data.ViewModels;
using webshop_owp.Models;

namespace webshop_owp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IProductsService _productsService;
        private readonly ShoppingCart _shoppingCart;
        private readonly IOrdersService _ordersService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(IProductsService productsService, ShoppingCart shoppingCart, IOrdersService ordersService, UserManager<ApplicationUser> userManager)
        {
            _productsService = productsService;
            _shoppingCart = shoppingCart;
            _ordersService = ordersService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string userRole = User.FindFirstValue(ClaimTypes.Role);

            var orders = await _ordersService.GetOrdersByUserIdAndRoleAsync(userId, userRole);
            return View(orders);
        }

        public IActionResult ShoppingCart()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            // Apply product-specific discounts to the total
            double total = 0;
            foreach (var item in items)
            {
                var price = item.Product.Price;
                if (item.Product.DiscountPercentage > 0)
                {
                    price = price * (1 - item.Product.DiscountPercentage / 100.0);
                }
                total += price * item.Amount;
            }

            var response = new ShoppingCartVM()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = total
            };

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddToShoppingCart(int id)
        {
            var item = await _productsService.GetByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.AddItemToCart(item);
            }
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, cartCount = _shoppingCart.GetShoppingCartItems().Count });
            }

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveFromShoppingCart(int id)
        {
            var item = await _productsService.GetByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.RemoveItemFromCart(item);
                TempData["Info"] = "Item removed from cart.";
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            var coupon = await _ordersService.GetCouponByCodeAsync(code);
            if (coupon != null)
            {
                return Json(new { success = true, discount = coupon.DiscountPercentage });
            }
            return Json(new { success = false, message = "Invalid or expired coupon code." });
        }

        // Checkout flow
        public async Task<IActionResult> Checkout()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            if (items.Count == 0) return RedirectToAction("Index", "Products");

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var checkoutVM = new CheckoutVM()
                {
                    FullName = user.FullName,
                    Email = user.Email
                };
                return View(checkoutVM);
            }

            return View(new CheckoutVM());
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder(CheckoutVM checkoutVM)
        {
            if (!ModelState.IsValid) return View("Checkout", checkoutVM);

            var items = _shoppingCart.GetShoppingCartItems();
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _ordersService.StoreOrderAsync(items, userId, checkoutVM.Email, checkoutVM.FullName, checkoutVM.Address, checkoutVM.City);
            await _shoppingCart.ClearShoppingCartAsync();

            TempData["Success"] = "Order placed successfully!";
            return View("OrderCompleted");
        }
    }
}
