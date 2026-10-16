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
        // TODO-01: one context per operation, read-only, ordered by name.
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Products.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct);
    }

    public async Task<int[]> CountConcurrentlyAsync(int parallelQueries, CancellationToken ct = default)
    {
        // TODO-02: overlapping queries, each with its own context.
        var tasks = Enumerable.Range(0, parallelQueries).Select(_ => Task.Run(async () =>
        {
            await using var db = await factory.CreateDbContextAsync(ct);
            return await db.Products.CountAsync(ct);
        }, ct));
        return await Task.WhenAll(tasks);
    }

    public async Task<bool> RenameAsync(int id, string newName, CancellationToken ct = default)
    {
        // TODO-06 (optional): load, change and save inside one short-lived context.
        await using var db = await factory.CreateDbContextAsync(ct);
        var product = await db.Products.FindAsync([id], ct);
        if (product is null) return false;
        product.Name = newName;
        await db.SaveChangesAsync(ct);
        return true;
    }
}
