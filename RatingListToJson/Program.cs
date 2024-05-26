using RatingListParser;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var mcrUrl = "http://labich.dk/mahjong/gamesmcr.php?length=long";
        var riichiUrl = "http://labich.dk/mahjong/gamesriichi.php?length=long";

        await DownloadAndParse(mcrUrl, "mcr_games_full.json");
        await DownloadAndParse(riichiUrl, "riichi_games_full.json");

    }

    private static async Task DownloadAndParse(string url, string filename)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0"); // server returns HTTP 555 without 

        var gamesHtml = await httpClient.GetStringAsync(url);

        var parser = new MahjongDkHtmlParser();

        var games = await parser.ParseGamesFromHtmlAsync(gamesHtml);

        var json = JsonSerializer.Serialize(games);
        Console.WriteLine($"downloaded and saved {games.Count()} from {url}");
        await File.WriteAllTextAsync(filename, json);
    }
}