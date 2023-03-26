using OnlineShop.Shared;
using System.Net.Http.Json;

namespace OnlineShop.Client.Services.ProductService;

public class ProductService : IProductService
{
    private readonly HttpClient _http;

    public List<Product> Products { get; set; } = new List<Product>();

    public ProductService(HttpClient http)
    {
        _http = http;
    }

    public async Task GetProducts()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product");
        if (result != null && result.Data != null)
        {
            Products = result.Data;

        }
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
}
