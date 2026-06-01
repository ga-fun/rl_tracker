namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadUpdateState(
	string? matchGuid,
	IReadOnlyList<Player>? players,
	Game? game
) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
	public IReadOnlyList<Player>? Players { get; } = players;
	public Game? Game { get; } = game;
}
