using FluentAssertions;
using RatingListParser;
using Xunit;

namespace RatingListParserTests;
public class ParserTests
{
    [Fact]
    public async Task ShouldParseExample()
    {
        var expected = new[] {
            new Game(new DateOnly(2024, 5, 16), "202405160060", 2, new[] { new Player("Player1", -151), new Player("Player2", 92), new Player("Player3", 90), new Player("Player4", -31) }),
            new Game(new DateOnly(2024, 5, 16), "202405160050", 2, new[] { new Player("Player5", 112), new Player("Player6", -129), new Player("Player7", -41), new Player("Player8", 58) }),
            new Game(new DateOnly(2024, 5, 16), "202405160040", 3, new[] { new Player("Player9", 132), new Player("Player10", -97), new Player("Player11", 58), new Player("Player12", -34), new Player("Player13", -59) }),
        };

        var example = File.ReadAllText("example.html");
        var actual = await new MahjongDkHtmlParser().ParseGamesFromHtmlAsync(example);

        actual.Should().BeEquivalentTo(expected);
    }
}
