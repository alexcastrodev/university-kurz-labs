using Microsoft.EntityFrameworkCore;

namespace BlazorDataAccessLab;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

public class ShopContext(DbContextOptions<ShopContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

/// <summary>
/// Test seam that simulates a slow data source. A component awaits it before querying,
/// so a test can hold the page "in flight" without any delay.
/// </summary>
public interface ILoadGate
{
    Task WaitAsync();
}

public sealed class ImmediateGate : ILoadGate
{
    public Task WaitAsync() => Task.CompletedTask;
}
