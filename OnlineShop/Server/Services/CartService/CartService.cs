using Microsoft.EntityFrameworkCore;
using OnlineShop.Server.Data;
using OnlineShop.Shared;

namespace OnlineShop.Server.Services.CartService;

public class CartService : ICartService
{
    private readonly DataContext _context;

    public CartService(DataContext context)
    {
        _context = context;
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
}
