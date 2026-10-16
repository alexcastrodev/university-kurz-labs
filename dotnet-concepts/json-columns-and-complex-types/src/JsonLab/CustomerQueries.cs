using Microsoft.EntityFrameworkCore;

namespace JsonLab;

public static class CustomerQueries
{
    public static Task<List<string>> NamesByCityAsync(ShopDbContext db, string city)
    {
        // TODO-03: Names (ordered) of customers whose Address.City equals city.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Task<List<string>> NamesWithTagAsync(ShopDbContext db, string tag)
    {
        // TODO-04: Names (ordered) of customers whose Tags contain tag.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Task<List<string>> NamesShippingToAsync(ShopDbContext db, string country)
    {
        // TODO-05: Names (ordered) of customers with at least one Shipping address in country.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Task MoveAsync(ShopDbContext db, int id, string newCity)
    {
        // TODO-06: Load the customer, replace its Address with a copy that has the new City
        //          (Address is a record) and save with SaveChanges.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Task<int> AppendToCityAsync(ShopDbContext db, string country, string suffix)
    {
        // TODO-07: One ExecuteUpdate that appends suffix to Address.City of every customer in
        //          country, without loading entities. Return the affected row count.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Task<int> SetFirstShippingCityAsync(ShopDbContext db, string city)
    {
        // TODO-08 (optional): ExecuteUpdate Shipping[0].City, but only for customers that have
        //          a shipping address. Without that filter SQLite creates a half-empty element.
        throw new NotImplementedException("Optional.");
    }

    public static Task<string?> StoredAddressJsonAsync(ShopDbContext db, int id)
    {
        // TODO-09 (optional): Return the raw JSON text of the Address column with
        //          Database.SqlQuery<string> (alias the column as Value).
        throw new NotImplementedException("Optional.");
    }
}
