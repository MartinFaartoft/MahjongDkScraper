public record Game(DateOnly DateOfGame, string Id, int NumberOfWinds, decimal Difficulty, IEnumerable<Player> Players);

public record Player(string Name, int Score, decimal OldRating, decimal NewRating);

public record DivisionGame(string[] Players);