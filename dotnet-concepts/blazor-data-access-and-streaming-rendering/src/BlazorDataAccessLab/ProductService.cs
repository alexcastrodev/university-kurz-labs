using Microsoft.EntityFrameworkCore;

namespace BlazorDataAccessLab;

public interface IProductService
{
    Task<List<Product>> ListAsync(CancellationToken ct = default);
    Task<int[]> CountConcurrentlyAsync(int parallelQueries, CancellationToken ct = default);
    Task<bool> RenameAsync(int id, string newName, CancellationToken ct = default);
}

public sealed class DbProductService(IDbContextFactory<ShopContext> factory) : IProductService
{
    public async Task<List<Product>> ListAsync(CancellationToken ct = default)
    {
        // TODO-01: create one context per call from the factory (and dispose it), run a read-only
        // query (no change tracking) and return the products ordered by name.
        await Task.CompletedTask;
        return [];
    }

    public async Task<int[]> CountConcurrentlyAsync(int parallelQueries, CancellationToken ct = default)
    {
        // TODO-02: start parallelQueries overlapping queries (Task.Run + Task.WhenAll), each
        // creating its own context from the factory, and return the product count of each one.
        await Task.CompletedTask;
        return [];
    }

    public async Task<bool> RenameAsync(int id, string newName, CancellationToken ct = default)
    {
        // TODO-06 (optional): load the product, change its name and save, all inside one
        // short-lived context. Return false when the id does not exist.
        await Task.CompletedTask;
        return false;
    }
}
