using BlazorSsrLab.Components;

var builder = WebApplication.CreateBuilder(args);

// Static SSR only: no AddInteractiveServerComponents, no WebAssembly.
builder.Services.AddRazorComponents();

var app = builder.Build();

// Bonus TODO-06: no code to write here. Find out which line makes a post without the token fail with 400.
app.UseAntiforgery();
app.MapRazorComponents<App>();

app.Run();

public partial class Program { }
