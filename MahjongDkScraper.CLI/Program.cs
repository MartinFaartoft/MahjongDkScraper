using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace MahjongDkScraper.CLI;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var connectionString = builder.Configuration["CONNECTION_STRING"] ?? throw new NullReferenceException("CONNECTION_STRING");
        var extractor = new MahjongDkDbExtractor(connectionString);
        
        await Extract(Ruleset.Mcr, extractor, "data/mcr_games_full.json");
        await Extract(Ruleset.Riichi, extractor, "data/riichi_games_full.json");
    }
    
    private static async Task Extract(Ruleset ruleset, MahjongDkDbExtractor extractor,  string filename)
    {
        
        var games = (await extractor.ExtractGames(ruleset)).ToList();
        var json = JsonSerializer.Serialize(games);
        Console.WriteLine($"downloaded and saved {games.Count} {ruleset} games");
        await File.WriteAllTextAsync(filename, json);
    }
}