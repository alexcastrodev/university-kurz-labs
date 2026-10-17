using BlazorSsrLab.Components;

var builder = WebApplication.CreateBuilder(args);

// Static SSR only: no AddInteractiveServerComponents, no WebAssembly.
builder.Services.AddRazorComponents();

var app = builder.Build();

app.UseAntiforgery();
app.MapRazorComponents<App>();

app.Run();

public partial class Program { }
