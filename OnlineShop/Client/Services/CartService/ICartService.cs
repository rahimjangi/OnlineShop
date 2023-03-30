using OnlineShop.Shared;

namespace OnlineShop.Client.Services.CartService;

public interface ICartService
{

    event Action OnChange;
    Task AddToCart(CartItem cartItem);
    Task<List<CartItem>> GetCartItems();
}
