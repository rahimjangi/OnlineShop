using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Shared;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    //[Column(TypeName = "decimal(18,2)")]
    //public decimal Price { get; set; }
    public Category? Category { get; set; }
    [ForeignKey("CategoryId")]
    public int CategoryId { get; set; }
    public bool Featured { get; set; }
    public List<ProductVariant> Varians { get; set; } = new List<ProductVariant>();
}
