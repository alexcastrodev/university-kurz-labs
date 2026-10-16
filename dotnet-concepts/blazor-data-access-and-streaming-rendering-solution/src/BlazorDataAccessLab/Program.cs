using BlazorDataAccessLab;
using BlazorDataAccessLab.Components;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents();

// A named shared-cache in-memory database. The keeper connection stays open for the whole
// lifetime of the host, otherwise SQLite drops the database when the last connection closes.
var connectionString = $"Data Source=shop-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
var keeper = new SqliteConnection(connectionString);
keeper.Open();
builder.Services.AddSingleton(keeper);

// TODO-00: register the context factory and the product service.
builder.Services.AddDbContextFactory<ShopContext>(o => o.UseSqlite(connectionString));
builder.Services.AddScoped<IProductService, DbProductService>();

builder.Services.AddSingleton<ILoadGate, ImmediateGate>();

var app = builder.Build();

using (var db = new ShopContext(new DbContextOptionsBuilder<ShopContext>().UseSqlite(keeper).Options))
{
    db.Database.EnsureCreated();
    db.Products.AddRange(
        new Product { Name = "Stapler", Price = 12.5m },
        new Product { Name = "Eraser", Price = 1.2m },
        new Product { Name = "Notebook", Price = 4m });
    db.SaveChanges();
}

app.UseAntiforgery();

// TODO-05: expose the same service as JSON, the shape a WebAssembly client would call.
app.MapGet("/api/products", (IProductService products, CancellationToken ct) => products.ListAsync(ct));

app.MapRazorComponents<App>();

app.Run();

public partial class Program { }
