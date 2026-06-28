using webshop_owp.Data.Cart;

namespace webshop_owp.Data.ViewModels
{
    public class ShoppingCartVM
    {
        public ShoppingCart ShoppingCart { get; set; } = null!;
        public decimal ShoppingCartTotal { get; set; }
    }
}