namespace RlStatsApi;

public sealed class PayloadUpdateState(
	string? matchGuid,
	List<Player>? players,
	Game? game
) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
	public List<Player>? Players { get; } = players;
	public Game? Game { get; } = game;
}
