namespace BlazorComponentLifecycleAndParameters.Tests;

public class LifecycleTests : BunitContext
{
    private sealed class FakeSource(Task<string> task) : IDataSource
    {
        public Task<string> LoadAsync() => task;
    }

    private sealed class CountingProducts : IProductService
    {
        public int Calls { get; private set; }

        public Task<string> GetNameAsync(int id)
        {
            Calls++;
            return Task.FromResult($"Product {id}");
        }
    }

    [Fact]
    public void Probe_logs_the_hooks_in_framework_order()
    {
        var log = new CallLog();
        Services.AddSingleton(log);

        var cut = Render<LifecycleProbe>(p => p.Add(x => x.Value, 1));

        Assert.Equal(
        [
            "SetParametersAsync", "OnInitialized", "OnInitializedAsync",
            "OnParametersSet", "OnParametersSetAsync",
            "OnAfterRender(first=True)", "OnAfterRenderAsync(first=True)"
        ], log.Entries);

        log.Clear();
        cut.Render(p => p.Add(x => x.Value, 2));

        Assert.Equal(
        [
            "SetParametersAsync", "OnParametersSet", "OnParametersSetAsync",
            "OnAfterRender(first=False)", "OnAfterRenderAsync(first=False)"
        ], log.Entries);
    }

    [Fact]
    public void AsyncLoader_renders_loading_first_and_the_data_after_the_task_completes()
    {
        var tcs = new TaskCompletionSource<string>();
        Services.AddSingleton<IDataSource>(new FakeSource(tcs.Task));

        var cut = Render<AsyncLoader>();

        Assert.Equal("Loading...", cut.Find("#state").TextContent);

        tcs.SetResult("hello");

        cut.WaitForAssertion(() => Assert.Equal("Loaded: hello", cut.Find("#state").TextContent));
    }

    [Fact]
    public void SelectableCard_invokes_the_callback_with_its_name()
    {
        string? received = null;

        var cut = Render<SelectableCard>(p => p
            .Add(x => x.Name, "Ada")
            .Add(x => x.OnSelected, (string name) => received = name));
        cut.Find("button").Click();

        Assert.Equal("Ada", received);
    }

    [Fact]
    public void SelectableCard_does_nothing_when_no_callback_is_supplied()
    {
        var cut = Render<SelectableCard>(p => p.Add(x => x.Name, "Ada"));

        cut.Find("button").Click();

        Assert.Equal("Ada", cut.Find("button").TextContent);
    }

    [Fact]
    public void SelectableCard_name_is_editor_required()
    {
        var property = typeof(SelectableCard).GetProperty(nameof(SelectableCard.Name))!;

        Assert.NotNull(Attribute.GetCustomAttribute(property, typeof(EditorRequiredAttribute)));
    }

    [Fact]
    public void CardPicker_refreshes_because_EventCallback_rerenders_the_owner()
    {
        var cut = Render<CardPicker>();

        cut.FindAll("button")[1].Click();

        Assert.Equal("Picked: Grace", cut.Find("#picked").TextContent);
    }

    [Fact]
    public void ProductView_reloads_only_when_the_id_changes()
    {
        var products = new CountingProducts();
        Services.AddSingleton<IProductService>(products);

        var cut = Render<ProductView>(p => p.Add(x => x.ProductId, 1).Add(x => x.Tags, [1]));
        Assert.Equal("Product 1", cut.Find("#name").TextContent);
        Assert.Equal(1, products.Calls);

        // A new list instance: OnParametersSet runs again, the id did not change.
        cut.Render(p => p.Add(x => x.ProductId, 1).Add(x => x.Tags, [1, 2]));
        Assert.Equal(1, products.Calls);

        cut.Render(p => p.Add(x => x.ProductId, 2).Add(x => x.Tags, [1, 2]));
        Assert.Equal(2, products.Calls);
        Assert.Equal("Product 2", cut.Find("#name").TextContent);
    }

    [Fact]
    public void ThemedLabel_uses_the_cascading_theme()
    {
        var cut = Render<ThemedLabel>(p => p.AddCascadingValue("dark"));

        Assert.Equal("label-dark", cut.Find("span").ClassList.Single());
    }

    [Fact]
    public void ThemedLabel_falls_back_to_light_without_a_cascading_value()
    {
        var cut = Render<ThemedLabel>();

        Assert.Equal("label-light", cut.Find("span").ClassList.Single());
    }

    [Fact]
    public async Task Ticker_updates_when_the_source_raises_from_another_thread()
    {
        var source = new TickSource();
        Services.AddSingleton(source);
        var cut = Render<Ticker>();

        await Task.Run(source.Raise);

        cut.WaitForAssertion(() => Assert.Equal("Ticks: 1", cut.Find("#ticks").TextContent));
    }

    [Fact]
    public async Task Ticker_unsubscribes_when_it_is_removed()
    {
        var source = new TickSource();
        Services.AddSingleton(source);
        Render<Ticker>();
        Assert.Equal(1, source.SubscriberCount);

        await DisposeComponentsAsync();

        Assert.Equal(0, source.SubscriberCount);
    }

    // Bonus tests

    [Fact]
    public void Badge_splats_unmatched_attributes()
    {
        var cut = Render<Badge>(p => p
            .Add(x => x.Text, "new")
            .AddUnmatched("data-id", "7"));

        Assert.Equal("7", cut.Find("span").GetAttribute("data-id"));
        Assert.Equal("new", cut.Find("span").TextContent);
    }

    [Fact]
    public void CounterBox_skips_renders_when_only_Other_changes_but_still_sets_parameters()
    {
        var cut = Render<CounterBox>(p => p.Add(x => x.Value, 1).Add(x => x.Other, 0));
        Assert.Equal(1, cut.Instance.RenderCount);

        cut.Render(p => p.Add(x => x.Value, 1).Add(x => x.Other, 5));
        Assert.Equal(1, cut.Instance.RenderCount);
        Assert.Equal(2, cut.Instance.ParametersSetCount);

        cut.Render(p => p.Add(x => x.Value, 2).Add(x => x.Other, 5));
        Assert.Equal(2, cut.Instance.RenderCount);
        Assert.Equal("Value: 2", cut.Find("#value").TextContent);
    }
}
