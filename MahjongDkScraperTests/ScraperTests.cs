using FluentAssertions;
using MahjongDkScraper;
using System.Globalization;
using Xunit;

namespace MahjongDkScraperTests;
public class ScraperTests
{
    [Fact]
    public async Task ShouldScrapeGamesExample()
    {
        CultureInfo.CurrentCulture = new CultureInfo("da-DK", false);

		var expected = new[] {
            new Game(new DateOnly(2024, 5, 16), "202405160060", 2, 30.05M, new[] { new Player("Player1", -151, 26.52M, 22.83M), new Player("Player2", 92, 54.38M, 56.36M), new Player("Player3", 90, -21.50M, -18.64M), new Player("Player4", -31, 60.79M, 59.64M) }),
            new Game(new DateOnly(2024, 5, 16), "202405160050", 2, 62.93M, new[] { new Player("Player5", 112, 50.96M, 53.88M), new Player("Player6", -129, 79.93M, 76.54M), new Player("Player7", -41, 50.29M, 49.43M), new Player("Player8", 58, 70.52M, 71.86M) }),
            new Game(new DateOnly(2024, 5, 16), "202405160040", 3, 21.24M, new[] { new Player("Player9", 132, 76.92M, 79.13M), new Player("Player10", -97, -19.87M, -21.50M), new Player("Player11", 58, 60.08M, 60.79M), new Player("Player12", -34, -39.03M, -38.75M), new Player("Player13", -59, 28.09M, 26.52M) }),
        };

        var example = File.ReadAllText("games_example.html");
        var actual = await new MahjongDkHtmlScraper().ScrapeGamesFromHtmlAsync(example);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ShouldScrapeDivisionExample()
    {
		var example = File.ReadAllText("division_example.html");
		var actual = await new MahjongDkHtmlScraper().ScrapeDivisionGamesFromHtmlAsync(example);
	}
}
