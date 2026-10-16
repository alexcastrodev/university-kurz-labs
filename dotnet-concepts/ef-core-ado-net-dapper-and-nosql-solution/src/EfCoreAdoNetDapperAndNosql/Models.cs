namespace EfCoreAdoNetDapperAndNosql;

public class Product
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = "";
    // Stored in the unit_price column: the name does not match the property.
    public double UnitPrice { get; set; }
}

public class Category
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public List<Product> Products { get; set; } = [];
}
