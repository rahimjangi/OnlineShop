using Blazored.LocalStorage;
using OnlineShop.Shared;

namespace OnlineShop.Client.Services.CartService;

public class CartService : ICartService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public event Action OnChange = null!;

    public CartService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }
    public async Task AddToCart(CartItem cartItem)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart == null)
        {
            cart = new List<CartItem>();
        }
        cart.Add(cartItem);
        await _localStorage.SetItemAsync("cart", cart);
    }

    public async Task<List<CartItem>> GetCartItems()
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart == null)
        {
            cart = new List<CartItem>();
        }
        return cart;
    }
}
