using Orders.Api.Orders;

var builder = WebApplication.CreateBuilder(args);

// Read once, before Build: OrderRules gets the number, not a configuration lookup.
var maxQuantity = builder.Configuration.GetValue("Orders:MaxQuantity", 10);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton(new OrderRules(maxQuantity));
builder.Services.AddSingleton<OrderStore>();
builder.Services.AddHttpClient<IInventoryClient, HttpInventoryClient>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Inventory:BaseUrl"]!));

var app = builder.Build();

app.MapOrdersEndpoints();

app.Run();
