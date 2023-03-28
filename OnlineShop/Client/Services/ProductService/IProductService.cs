using OnlineShop.Shared;

namespace OnlineShop.Client.Services.ProductService;

public interface IProductService
{
    event Action? ProductChanged;
    List<Product> Products { get; set; }
    string Message { get; set; }

    Task GetProducts(string? categoryUrl = null);

    Task<ServiceResponse<Product>> GetProductAsync(int productId);

    Task SearchProducts(string searchText);
    Task<List<string>> GetProductSearchSuggestions(string searchText);
}
