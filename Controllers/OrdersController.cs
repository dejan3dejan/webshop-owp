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

        public async Task<IActionResult> AddToShoppingCart(int id)
        {
            var item = await _productsService.GetByIdAsync(id);

            bool added = false;
            if (item != null)
            {
                added = _shoppingCart.AddItemToCart(item);
                if (!added)
                {
                    TempData["Error"] = "Cannot add more items. Stock limit reached.";
                }
            }
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = added, message = added ? "" : "Stock limit reached.", cartCount = _shoppingCart.GetShoppingCartItems().Count });
            }

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> RemoveFromShoppingCart(int id)
        {
            var item = await _productsService.GetByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.RemoveItemFromCart(item);
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, cartCount = _shoppingCart.GetShoppingCartItems().Count });
            }

            return RedirectToAction(nameof(ShoppingCart));
        }

        public async Task<IActionResult> ClearItemFromShoppingCart(int id)
        {
            var item = await _productsService.GetByIdAsync(id);

            if (item != null)
            {
                _shoppingCart.ClearItemFromCart(item);
                TempData["Info"] = "Product removed from cart.";
            }
            return RedirectToAction(nameof(ShoppingCart));
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            var coupon = await _ordersService.GetCouponByCodeAsync(code);
            if (coupon != null)
            {
                HttpContext.Session.SetString("AppliedCoupon", code);
                HttpContext.Session.SetInt32("CouponDiscount", (int)coupon.DiscountPercentage);
                
                var total = _shoppingCart.GetShoppingCartTotal();
                var discountAmount = total * (coupon.DiscountPercentage / 100.0);
                var newTotal = total - discountAmount;

                return Json(new { 
                    success = true, 
                    discount = coupon.DiscountPercentage,
                    newTotal = newTotal.ToString("c")
                });
            }
            return Json(new { success = false, message = "Invalid or expired coupon code." });
        }

        // Checkout flow
        public async Task<IActionResult> Checkout()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            if (items.Count == 0) return RedirectToAction("Index", "Products");

            double total = _shoppingCart.GetShoppingCartTotal();
            string couponCode = HttpContext.Session.GetString("AppliedCoupon");
            
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon = await _ordersService.GetCouponByCodeAsync(couponCode);
                if (coupon != null)
                {
                    total -= total * (coupon.DiscountPercentage / 100.0);
                    ViewBag.CouponCode = couponCode;
                    ViewBag.DiscountPercentage = coupon.DiscountPercentage;
                }
            }

            ViewBag.ShoppingCartTotal = total;
            ViewBag.ShoppingCartItems = items;

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

            // Get coupon from session
            string couponCode = HttpContext.Session.GetString("AppliedCoupon");

            await _ordersService.StoreOrderAsync(items, userId, checkoutVM.Email, checkoutVM.FullName, checkoutVM.Address, checkoutVM.City, couponCode);
            await _shoppingCart.ClearShoppingCartAsync();

            // Clear coupon session
            HttpContext.Session.Remove("AppliedCoupon");
            HttpContext.Session.Remove("CouponDiscount");

            TempData["Success"] = "Order placed successfully!";
            return View("OrderCompleted");
        }
    }
}
