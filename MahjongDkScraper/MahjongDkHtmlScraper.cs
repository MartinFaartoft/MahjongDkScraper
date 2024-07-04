using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace MahjongDkScraper;

public class MahjongDkHtmlScraper
{

	private static readonly int[] scoreIndices = [3, 6, 9, 12, 15, 18, 21];
    private static readonly int[] oldRatingIndices = scoreIndices.Select(n => n + 1).ToArray();
	private static readonly int[] newRatingIndices = scoreIndices.Select(n => n + 2).ToArray();

	public async Task<IEnumerable<DivisionGame>> ScrapeDivisionGamesFromHtmlAsync(string html)
	{
		var parser = new HtmlParser();
		var doc = await parser.ParseDocumentAsync(html);
        var divisionGames = doc.QuerySelectorAll("div[tabindex=0]").Chunk(2);

        return divisionGames.Select(ParseDivisionChunk);
	}

	private DivisionGame ParseDivisionChunk(IElement[] divisionChunk)
	{
		// row 1 = heading
        // row 2 = <br> separated list of games
	}

	public async Task<IEnumerable<Game>> ScrapeGamesFromHtmlAsync(string html)
    {
        var parser = new HtmlParser();
        var doc = await parser.ParseDocumentAsync(html);
        var games = doc.QuerySelectorAll("tr").ToArray()[4..^1].Chunk(2);

        return games.Select(ParseChunk).ToArray();
    }

    private Game ParseChunk(IElement[] gameRows)
    {
        // extract only text contents
        var headers = gameRows[0].Children.Select(c => c.TextContent.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToArray();
        var scores = gameRows[1].Children.Select(c => c.TextContent.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToArray();

        var id = headers[1];
        var date = DateOnly.ParseExact(id[0..8], "yyyyMMdd");
        var numberOfWinds = int.Parse(scores[1]);
        var difficulty = decimal.Parse(scores[2]);

        var playerNames = headers[2..];

		// skip TD's with old and new rating
		var playerScoreIndexes = scoreIndices.Take(playerNames.Length);
        var playerOldRatingIndexes = oldRatingIndices.Take(playerNames.Length);
		var playerNewRatingIndexes = newRatingIndices.Take(playerNames.Length);
		var playerScores = playerScoreIndexes.Select(i => int.Parse(scores[i])).ToArray();
        var playerOldRatings = playerOldRatingIndexes.Select(i => decimal.Parse(scores[i])).ToArray();
		var playerNewRatings = playerNewRatingIndexes.Select(i => decimal.Parse(scores[i])).ToArray();

        var players = playerNames.Select((n, i) => new Player(n, playerScores[i], playerOldRatings[i], playerNewRatings[i])).ToArray();
        
        return new Game(date, id, numberOfWinds, difficulty, players);
    }
}