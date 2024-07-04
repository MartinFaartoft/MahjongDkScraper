using AngleSharp.Dom;
using MahjongDkScraper;
using System.Globalization;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
		CultureInfo.CurrentCulture = new CultureInfo("da-DK", false);

		var mcrGamesUrl = "http://labich.dk/mahjong/gamesmcr.php?length=long";
        var mcrDivisionGamesUrl = "https://mahjong.dk/divisionskampe-i-mcr";
		var riichiGamesUrl = "http://labich.dk/mahjong/gamesriichi.php?length=long";
		var riichiDivisionGamesUrl = "https://mahjong.dk/divisionskampe-i-riichi";

		await ScrapeGames(mcrGamesUrl, "data/mcr_games_full.json");
        await ScrapeGames(riichiGamesUrl, "data/riichi_games_full.json");

        await ScrapeDivisionGames(mcrDivisionGamesUrl, "data/mcr_remaining_division_games.json");
		await ScrapeDivisionGames(riichiDivisionGamesUrl, "data/riichi_remaining_division_games.json");
	}

	private static async Task ScrapeDivisionGames(string url, string filename)
	{
		var httpClient = new HttpClient();
		var divisionHtml = await httpClient.GetStringAsync(url);

		var scraper = new MahjongDkHtmlScraper();

		var divisionGames = await scraper.ScrapeDivisionGamesFromHtmlAsync(divisionHtml);

		var json = JsonSerializer.Serialize(divisionGames);
		Console.WriteLine($"downloaded and saved {divisionGames.Count()} division games from {url}");
		await File.WriteAllTextAsync(filename, json);
	}

	private static async Task ScrapeGames(string url, string filename)
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