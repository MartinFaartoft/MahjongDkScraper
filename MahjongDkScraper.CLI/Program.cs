using MahjongDkScraper;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var mcrUrl = "http://labich.dk/mahjong/gamesmcr.php?length=long";
        var riichiUrl = "http://labich.dk/mahjong/gamesriichi.php?length=long";

        await Scrape(mcrUrl, "data/mcr_games_full.json");
        await Scrape(riichiUrl, "data/riichi_games_full.json");
    }

    private static async Task Scrape(string url, string filename)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0"); // server returns HTTP 555 without 

        var gamesHtml = await httpClient.GetStringAsync(url);

        var scraper = new MahjongDkHtmlScraper();

        var games = await scraper.ScrapeGamesFromHtmlAsync(gamesHtml);

        var json = JsonSerializer.Serialize(games);
        Console.WriteLine($"downloaded and saved {games.Count()} from {url}");
        await File.WriteAllTextAsync(filename, json);
    }
}