using Microsoft.EntityFrameworkCore;
using webshop_owp.Models;

namespace webshop_owp.Data.Cart
{
    public class ShoppingCart
    {
        public AppDbContext _context { get; set; }

        public string ShoppingCartId { get; set; } = string.Empty;
        public List<ShoppingCartItem>? ShoppingCartItems { get; set; }

        public ShoppingCart(AppDbContext context)
        {
            _context = context;
        }
        
        public static ShoppingCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>().HttpContext!.Session;
            var context = services.GetRequiredService<AppDbContext>();
            
            string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
            session.SetString("CartId", cartId);

            return new ShoppingCart(context) { ShoppingCartId = cartId };
        }

        public bool AddItemToCart(Product product)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.Product.Id == product.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                if (product.StockAmount > 0)
                {
                    shoppingCartItem = new ShoppingCartItem()
                    {
                        ShoppingCartId = ShoppingCartId,
                        Product = product,
                        Amount = 1
                    };
                    _context.ShoppingCartItems.Add(shoppingCartItem);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (shoppingCartItem.Amount < product.StockAmount)
                {
                    shoppingCartItem.Amount++;
                }
                else
                {
                    return false;
                }
            }
            _context.SaveChanges();
            return true;
        }

        public void RemoveItemFromCart(Product product)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.Product.Id == product.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                }
                else
                {
                    _context.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            _context.SaveChanges();
        }

        public void ClearItemFromCart(Product product)
        {
            var shoppingCartItem = _context.ShoppingCartItems.FirstOrDefault(n => n.Product.Id == product.Id && n.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem != null)
            {
                _context.ShoppingCartItems.Remove(shoppingCartItem);
            }
            _context.SaveChanges();
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ?? (ShoppingCartItems = _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Include(n => n.Product).ToList());
        }

        public decimal GetShoppingCartTotal()
        {
            var items = _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).Include(n => n.Product).ToList();
            decimal total = 0;
            foreach (var item in items)
            {
                var price = item.Product.Price;
                if (item.Product.DiscountPercentage > 0)
                {
                    price = price * (1 - item.Product.DiscountPercentage / 100m);
                }
                total += price * item.Amount;
            }
            return total;
        }

        public async Task ClearShoppingCartAsync()
        {
            var items = await _context.ShoppingCartItems.Where(n => n.ShoppingCartId == ShoppingCartId).ToListAsync();
            _context.ShoppingCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}