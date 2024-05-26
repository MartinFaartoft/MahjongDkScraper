using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace RatingListParser;

public class MahjongDkHtmlParser
{
    public async Task<IEnumerable<Game>> ParseGamesFromHtmlAsync(string html)
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

        var playerNames = headers[2..];
        
        // skip TD's with old and new rating
        var playerScoreIndexes = new int[] { 3, 6, 9, 12, 15, 18, 21 }.Take(playerNames.Length);
        var playerScores = playerScoreIndexes.Select(i => int.Parse(scores[i])).ToArray();

        var players = playerNames.Zip(playerScores, (first, second) => new Player(first, second)).ToArray();
        
        return new Game(date, id, numberOfWinds, players);
    }
}