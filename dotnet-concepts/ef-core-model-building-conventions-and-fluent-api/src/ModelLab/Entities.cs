using System.ComponentModel.DataAnnotations;

namespace ModelLab;

public enum ProductStatus { Draft, Active, Retired }

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    [MaxLength(50)]
    public string Slug { get; set; } = "";

    public List<Product> Products { get; set; } = [];
}

public class Product
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = "";

    public string Sku { get; set; } = "";
    public decimal Price { get; set; }
    public ProductStatus Status { get; set; }
    public string? Note { get; set; }
    public string? CachedLabel { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
