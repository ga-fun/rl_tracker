namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadUpdateState(
	string matchGuid,
	IReadOnlyList<Player> players,
	Game game
) : Payload(matchGuid)
{
	public IReadOnlyList<Player> Players { get; } = players;
	public Game Game { get; } = game;
}
