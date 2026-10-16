using Microsoft.EntityFrameworkCore;

namespace RelationshipsLab;

public class ShopService(ShopContext db)
{
    public Task<int> GetReviewProductIdAsync(int reviewId) =>
        db.Reviews.Where(r => r.Id == reviewId)
            .Select(r => EF.Property<int>(r, "ProductId"))
            .SingleAsync();

    public async Task DetachReviewAsync(int productId, int reviewId)
    {
        var product = await db.Products.Include(p => p.Reviews).SingleAsync(p => p.Id == productId);
        product.Reviews.RemoveAll(r => r.Id == reviewId);
        await db.SaveChangesAsync();
    }

    public async Task DeleteSupplierAsync(int id)
    {
        db.Remove(await db.Suppliers.SingleAsync(s => s.Id == id));
        await db.SaveChangesAsync();
    }
}
