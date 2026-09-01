using System.Globalization;
using MySqlConnector;

namespace MahjongDkScraper;

public class MahjongDkDbExtractor
{
    private readonly string _connectionString;

    public MahjongDkDbExtractor(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task<IEnumerable<Game>> ExtractGames(Ruleset ruleset)
    {
        await using var conn = new MySqlConnection(_connectionString);
        await conn.OpenAsync();
        
        var players = new Dictionary<int, BasePlayer>();
        
        await using var cmdPlayers = new MySqlCommand("SELECT p_id, p_name, p_private FROM players", conn);
        await using (var readerPlayers = await cmdPlayers.ExecuteReaderAsync())
        {
            while (readerPlayers.Read())
            {
                bool isHidden = readerPlayers.GetBoolean("p_private");
                var id = readerPlayers.GetInt32("p_id"); 
                var p = new BasePlayer(
                    id,
                    isHidden ? $"Hidden player ({id})" : readerPlayers.GetString("p_name"));
                players.Add(p.Id, p);
            }
        }

        var tableName = ruleset == Ruleset.Mcr ? "games_mcr" : "games_riichi";

        var games = new List<Game>();
        await using var cmdGames = new MySqlCommand($"SELECT * FROM {tableName}", conn);
        await using (var readerGames = await cmdGames.ExecuteReaderAsync())
        {
            while (readerGames.Read())
            {
                var id = readerGames.GetInt64("game_id").ToString(CultureInfo.InvariantCulture);
                var date = ExtractDate(id);
                var g = new Game(
                    date,
                    id,
                    readerGames.GetInt32("winds"),
                    readerGames.GetDecimal("difficulty"),
                    ReadPlayers(readerGames, players)
                );

                games.Add(g);
            }
        }

        return games;
    }
    
    private static IEnumerable<Player> ReadPlayers(MySqlDataReader reader, Dictionary<int, BasePlayer> players)
    {
        var player4Id = reader.GetInt32("p4_id");
        int playerCount = player4Id == 0 ? 3 : reader.GetInt32("num_players"); // a single 2026 MCR game has 3 players, but num_players=4 
        var parsedPlayers = Enumerable.Range(1, playerCount)
            .Select(n => new Player(
                Name: players[reader.GetInt32($"p{n}_id")].Name,
                Score: reader.GetInt32($"p{n}_score"),
                OldRating: reader.GetDecimal($"p{n}_rating_old"),
                NewRating: reader.GetDecimal($"p{n}_rating_new")));

        return parsedPlayers.ToList();
    }

    private static DateOnly ExtractDate(string value)
    {
        return DateOnly.ParseExact(value.AsSpan(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture);
    }
    
    private record BasePlayer(int Id, string Name);
}