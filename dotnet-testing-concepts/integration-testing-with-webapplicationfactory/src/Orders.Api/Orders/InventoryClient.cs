namespace Orders.Api.Orders;

public interface IInventoryClient
{
    Task<int> GetAvailableAsync(string sku, CancellationToken ct);
}

// Calls the inventory service over HTTP. No such service exists while the tests run.
public sealed class HttpInventoryClient(HttpClient http) : IInventoryClient
{
    public async Task<int> GetAvailableAsync(string sku, CancellationToken ct) =>
        await http.GetFromJsonAsync<int>($"/stock/{Uri.EscapeDataString(sku)}", ct);
}
