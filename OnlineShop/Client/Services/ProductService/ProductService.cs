using OnlineShop.Shared;
using System.Net.Http.Json;

namespace OnlineShop.Client.Services.ProductService;

public class ProductService : IProductService
{
    private readonly HttpClient _http;

    public event Action? ProductChanged;

    public List<Product> Products { get; set; } = new List<Product>();
    public string Message { get; set; } = "Loading products...";

    public ProductService(HttpClient http)
    {
        _http = http;
    }

    public async Task GetProducts(string? categoryUrl = null)
    {
        var result = categoryUrl == null ?
            await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product") :
            await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryUrl}");
        if (result != null && result.Data != null)
        {
            Products = result.Data;

        }
        ProductChanged?.Invoke();
    }

    public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
    {
        ServiceResponse<Product> productResult = new ServiceResponse<Product>();
        string urlString = $"api/Product/{productId.ToString()}";
        var result = await _http.GetFromJsonAsync<ServiceResponse<Product>>(urlString);
        if (result != null && result.Data != null)
        {
            productResult.Data = result.Data;

        }
        else
        {
            productResult.Success = false;
            productResult.Message = "Product does not exist!";
        }
        return productResult;
    }

    public async Task SearchProducts(string searchText)
    {
        var result = await _http
            .GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/search/{searchText}");
        if (result != null && result.Data != null) { Products = result.Data; }
        if (Products.Count == 0) Message = "No product found!";
        ProductChanged?.Invoke();
    }

    public async Task<List<string>> GetProductSearchSuggestions(string searchText)
    {
        var returnStrings = new List<string>();
        var result = await _http
            .GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestions/{searchText}");
        if (result != null && result.Data != null)
        {
            returnStrings.AddRange(result.Data);
        }
        return returnStrings;
    }
}
