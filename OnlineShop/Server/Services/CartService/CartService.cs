using Microsoft.EntityFrameworkCore;
using OnlineShop.Server.Data;
using OnlineShop.Shared;
using System.Security.Claims;

namespace OnlineShop.Server.Services.CartService;

public class CartService : ICartService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CartService(DataContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    private int GetUserId()
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            return int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        return 0;
    }


    public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
    {
        var result = new ServiceResponse<List<CartProductResponse>>()
        {
            Data = new List<CartProductResponse>()
        };

        foreach (var cartItem in cartItems)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartItem.ProductId);
            if (product == null)
            {
                continue;
            }

            var productVariants = await _context.ProductVariants
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(v => v.ProductId == cartItem.ProductId && v.ProductTypeId == cartItem.ProductTypeId);
            if (productVariants == null)
            {
                continue;
            }
            if (productVariants.ProductType == null)
            {
                continue;
            }

            var cartProduct = new CartProductResponse()
            {
                ProductId = product.Id,
                Title = product.Title,
                ImageUrl = product.ImageUrl,
                Price = productVariants.Price,
                ProductType = productVariants.ProductType.Name,
                ProductTypeId = productVariants.ProductTypeId,
                Quantity = cartItem.Quantity
            };
            result.Data.Add(cartProduct);
        }
        return result;
    }

    public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItems(List<CartItem> cartItems)
    {
        var authUserId = GetUserId();
        if (authUserId != 0)
        {
            cartItems.ForEach(cartItem => cartItem.UserId = authUserId);
        }
        await _context.CartItems.AddRangeAsync(cartItems);
        await _context.SaveChangesAsync();
        var result = await GetCartProducts(await _context.CartItems
            .Where(x => x.UserId == authUserId)
            .ToListAsync());

        return result;
    }

    public async Task<ServiceResponse<int>> GetCartItemsCount()
    {
        var count = (await _context.CartItems.Where(
            ci => ci.UserId == GetUserId()).ToListAsync()).Count;
        return new ServiceResponse<int> { Data = count };
    }
}
