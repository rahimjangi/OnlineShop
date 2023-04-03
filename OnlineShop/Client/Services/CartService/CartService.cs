using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using OnlineShop.Shared;
using System.Net.Http.Json;

namespace OnlineShop.Client.Services.CartService;

public class CartService : ICartService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public event Action OnChange = null!;

    public CartService(HttpClient http,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    private async Task<bool> IsUserAuthenticated()
    {
        var currentUser = await _authStateProvider.GetAuthenticationStateAsync();
        if (currentUser != null && currentUser.User.Identity != null && currentUser.User.Identity.IsAuthenticated)
        {
            return true;
        }
        return false;
    }
    public async Task AddToCart(CartItem cartItem)
    {
        if (await IsUserAuthenticated())
        {
            Console.WriteLine($"User  is authenticated");
        }
        else
        {
            Console.WriteLine($"User  is not authenticated");
        }
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
        await GetCartItemsCount();
    }

    public async Task<List<CartItem>> GetCartItems()
    {
        await GetCartItemsCount();
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
                await GetCartItemsCount();
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

    public async Task StoreCartItems(bool emptyLocalCart = false)
    {
        var localCart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
        if (localCart == null)
        {
            return;
        }
        var result = await _http.PostAsJsonAsync("api/cart", localCart);
        var cartProducts = await result.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
        if (emptyLocalCart && cartProducts != null && cartProducts.Success)
        {
            await _localStorage.RemoveItemAsync("cart");
        }
    }

    public async Task GetCartItemsCount()
    {
        var count = 0;
        if (await IsUserAuthenticated())
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");

            if (result != null)
            {
                count = result.Data;
            }
            await _localStorage.SetItemAsync<int>("cartItemsCount", count);
        }
        else
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart != null && cart.Count > 0)
            {
                count = cart.Count;
            }
            await _localStorage.SetItemAsync<int>("cartItemsCount", count);
        }
        OnChange.Invoke();
    }
}
