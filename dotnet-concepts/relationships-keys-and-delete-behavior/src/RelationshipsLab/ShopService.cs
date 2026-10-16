using Microsoft.EntityFrameworkCore;

namespace RelationshipsLab;

public class ShopService(ShopContext db)
{
    public Task<int> GetReviewProductIdAsync(int reviewId)
    {
        // TODO-05: Review has no ProductId property, only the Product navigation. Read the
        //   shadow foreign key column for the given review with EF.Property<int>.
        throw new NotImplementedException("Not implemented yet.");
    }

    public Task DetachReviewAsync(int productId, int reviewId)
    {
        // TODO-06: Load the product with its reviews, remove the review from product.Reviews
        //   and save. Do NOT call Remove on the review: EF Core must delete it because a
        //   review cannot exist without a product (orphan deletion).
        throw new NotImplementedException("Not implemented yet.");
    }

    public async Task DeleteSupplierAsync(int id)
    {
        db.Remove(await db.Suppliers.SingleAsync(s => s.Id == id));
        await db.SaveChangesAsync();
    }
}
