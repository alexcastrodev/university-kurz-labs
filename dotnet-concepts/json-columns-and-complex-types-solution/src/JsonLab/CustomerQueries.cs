using Microsoft.EntityFrameworkCore;

namespace JsonLab;

public static class CustomerQueries
{
    public static Task<List<string>> NamesByCityAsync(ShopDbContext db, string city) =>
        db.Customers.Where(c => c.Address.City == city).OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();

    public static Task<List<string>> NamesWithTagAsync(ShopDbContext db, string tag) =>
        db.Customers.Where(c => c.Tags.Contains(tag)).OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();

    public static Task<List<string>> NamesShippingToAsync(ShopDbContext db, string country) =>
        db.Customers.Where(c => c.Shipping.Any(s => s.Country == country)).OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();

    public static async Task MoveAsync(ShopDbContext db, int id, string newCity)
    {
        var customer = await db.Customers.SingleAsync(c => c.Id == id);
        customer.Address = customer.Address with { City = newCity };
        await db.SaveChangesAsync();
    }

    public static Task<int> AppendToCityAsync(ShopDbContext db, string country, string suffix) =>
        db.Customers
            .Where(c => c.Address.Country == country)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Address.City, c => c.Address.City + suffix));

    public static Task<int> SetFirstShippingCityAsync(ShopDbContext db, string city) =>
        db.Customers
            .Where(c => c.Shipping.Count > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Shipping[0].City, city));

    public static Task<string?> StoredAddressJsonAsync(ShopDbContext db, int id) =>
        db.Database.SqlQuery<string>($"SELECT Address AS Value FROM Customers WHERE Id = {id}").SingleOrDefaultAsync();
}
