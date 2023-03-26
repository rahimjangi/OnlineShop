using Microsoft.EntityFrameworkCore;
using OnlineShop.Server.Data;
using OnlineShop.Shared;

namespace OnlineShop.Server.Services.ProductService;

public class ProductService : IProductService
{
    private readonly DataContext _context;

    public ProductService(DataContext _context)
    {
        this._context = _context;
    }

    public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        var response = new ServiceResponse<Product>();
        if (product != null)
        {
            response.Data = product;
        }
        else
        {
            response.Data = null;
            response.Message = "Sorry this product does not exist";
        }
        return response;
    }

    public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
    {
        var response = new ServiceResponse<List<Product>>()
        {
            Data = await _context.Products.ToListAsync(),
            Success = true,
            Message = "OK"
        };
        return response;
    }
}
