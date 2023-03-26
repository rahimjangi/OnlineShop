using OnlineShop.Shared;

namespace OnlineShop.Client.Services.ProductService;

public interface IProductService
{
    List<Product> Products { get; set; }
    Task GetProducts();

    Task<ServiceResponse<Product>> GetProductAsync(int productId);
}
