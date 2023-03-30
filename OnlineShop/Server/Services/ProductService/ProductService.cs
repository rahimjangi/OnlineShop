using Microsoft.EntityFrameworkCore;
using OnlineShop.Server.Data;
using OnlineShop.Shared;
using OnlineShop.Shared.Dtos;

namespace OnlineShop.Server.Services.ProductService;

public class ProductService : IProductService
{
    private readonly DataContext _context;

    public ProductService(DataContext _context)
    {
        this._context = _context;
    }

    public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
    {
        var response = new ServiceResponse<List<Product>>()
        {
            Data = await _context.Products
            .Where(p => p.Featured)
            .Include(p => p.Varians)
            .ToListAsync()
        };

        return response;
    }

    public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Varians)
            .ThenInclude(p => p.ProductType)
            .FirstOrDefaultAsync(p => p.Id == productId);

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
            Data = await _context.Products
                .Include(p => p.Varians)
                .ToListAsync(),
            Success = true,
            Message = "OK"
        };
        return response;
    }

    public async Task<ServiceResponse<List<Product>>> GetProductsByCategoryAsync(string categoryUri)
    {
        var result = await _context.Products
            .Where(x => x.Category != null && x.Category.Url.ToLower().Equals(categoryUri.ToLower()))
            .Include(p => p.Varians)
            .ToListAsync();
        var response = new ServiceResponse<List<Product>>()
        {

            Data = result
        };
        return response;
    }

    public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
    {
        var products = await FindProductBySearchText(searchText);
        List<string> suggestions = new List<string>();

        foreach (var product in products)
        {
            if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            {
                suggestions.Add(product.Title);
            }
            if (product.Description != null)
            {
                var punctuation = product.Description.Where(char.IsPunctuation)
                    .Distinct()
                    .ToArray();
                var words = product.Description.Split()
                    .Select(x => x.Trim(punctuation));
                foreach (var word in words)
                {
                    if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                        && !suggestions.Contains(word))
                    {
                        suggestions.Add(word);
                    }
                }
            }
        }
        var result = new ServiceResponse<List<string>>()
        {
            Data = suggestions
        };
        return result;
    }

    public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page)
    {
        var pageResults = 2f;
        var pageCount = Math.Ceiling((await FindProductBySearchText(searchText)).Count / pageResults);
        var products = await _context.Products
                    .Where(p => p.Title.ToLower().Contains(searchText) || p.Description.ToLower().Contains(searchText))
                    .Include(p => p.Varians)
                    .Skip((page - 1) * Convert.ToInt32(pageResults))
                    .Take(Convert.ToInt32(pageResults))
                    .ToListAsync();

        var response = new ServiceResponse<ProductSearchResult>()
        {
            Data = new ProductSearchResult
            {
                Products = products,
                Pages = Convert.ToInt32(pageResults),
                CurrentPage = page
            }
        };
        return response;
    }

    private async Task<List<Product>> FindProductBySearchText(string searchText)
    {
        return await _context.Products
                    .Where(p => p.Title.ToLower().Contains(searchText) || p.Description.ToLower().Contains(searchText))
                    .Include(p => p.Varians)
                    .ToListAsync();
    }
}
