using OnlineShop.Shared;
using OnlineShop.Shared.Dtos;

namespace OnlineShop.Server.Services.ProductService;

public interface IProductService
{
    Task<ServiceResponse<List<Product>>> GetProductsAsync();
    Task<ServiceResponse<List<Product>>> GetProductsByCategoryAsync(string categoryUri);
    Task<ServiceResponse<Product>> GetProductAsync(int productId);

    Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page);

    Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText);

    Task<ServiceResponse<List<Product>>> GetFeaturedProducts();
}
