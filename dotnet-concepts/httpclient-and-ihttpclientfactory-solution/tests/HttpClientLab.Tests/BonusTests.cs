using Microsoft.Extensions.DependencyInjection;

namespace HttpClientLab.Tests;

public sealed class BonusTests
{
    private static GamesClient Client(FakeHandler fake)
    {
        var services = new ServiceCollection();
        services.AddGamesClient(new Uri("http://games.test")).ConfigurePrimaryHttpMessageHandler(() => fake);
        return services.BuildServiceProvider().GetRequiredService<GamesClient>();
    }

    [Fact]
    public async Task ShouldStreamGamesAsTheyAreDeserialized()
    {
        var client = Client(FakeHandler.Json(new[] { new Game(1, "A"), new Game(2, "B") }));

        var titles = new List<string>();
        await foreach (var game in client.StreamGamesAsync())
        {
            titles.Add(game!.Title);
        }

        Assert.Equal(["A", "B"], titles);
    }

    [Fact]
    public async Task ShouldApplyAPerCallTimeoutShorterThanTheClientTimeout()
    {
        var client = Client(FakeHandler.Hanging());

        await Assert.ThrowsAsync<GameServiceTimeoutException>(
            () => client.GetGameAsync(1, TimeSpan.FromMilliseconds(100)));
    }
}
