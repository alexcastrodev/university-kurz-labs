namespace RelationshipsLab;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Product> Products { get; } = [];
}

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Product> Products { get; } = [];
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public List<Tag> Tags { get; } = [];
    public List<Review> Reviews { get; } = [];
}

// No foreign key property on purpose: EF Core creates a shadow ProductId column.
public class Review
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public Product Product { get; set; } = null!;
}

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Product> Products { get; } = [];
}

public class ProductTag
{
    public int ProductId { get; set; }
    public int TagId { get; set; }
    public DateTime AddedOn { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public List<Order> Orders { get; } = [];
}

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = "";
    public Customer Customer { get; set; } = null!;
}

public class OrderLine
{
    public int OrderId { get; set; }
    public int LineNo { get; set; }
    public string Sku { get; set; } = "";
}
