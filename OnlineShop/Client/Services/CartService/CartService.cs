using Blazored.LocalStorage;
using OnlineShop.Shared;
using System.Net.Http.Json;

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
        var sameItem = cart
            .Find(p => p.ProductId == cartItem.ProductId && p.ProductTypeId == cartItem.ProductTypeId);
        if (sameItem == null)
        {
            cart.Add(cartItem);
        }
        else
        {
            sameItem.Quantity += cartItem.Quantity;
        }
        await _localStorage.SetItemAsync("cart", cart);
        OnChange.Invoke();
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

    public async Task<List<CartProductResponse>> GetCartProducts()
    {
        var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");

        if (cartItems == null || cartItems.Count == 0)
        {
            return null!;
        }
        else
        {
            var productsCartItems = await _http.PostAsJsonAsync("api/cart/products", cartItems);
            var cartProducts = await productsCartItems.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
            if (cartProducts != null && cartProducts.Data != null)
            {
                return cartProducts.Data;
            }
            else
            {
                return null!;
            }
        }


    }

    public async Task RemoveProductFromCart(int productId, int productTypeId)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart == null)
        {
            return;
        }
        else
        {
            var cartItem = cart.FirstOrDefault(c => c.ProductTypeId == productTypeId && c.ProductId == productId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                await _localStorage.SetItemAsync("cart", cart);
                OnChange.Invoke();
            }
        }
    }

    public async Task UpdateQuantity(CartProductResponse product)
    {
        var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (cart == null)
        {
            return;
        }
        else
        {
            var cartItem = cart.FirstOrDefault(c => c.ProductTypeId == product.ProductTypeId && c.ProductId == product.ProductId);
            if (cartItem != null)
            {
                cartItem.Quantity = product.Quantity;
                await _localStorage.SetItemAsync("cart", cart);

            }
        }
    }
}
