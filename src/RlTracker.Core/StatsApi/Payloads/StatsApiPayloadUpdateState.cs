namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadUpdateState(
	string? matchGuid,
	List<StatsApiPlayer>? players,
	StatsApiGame? game
) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
	public List<StatsApiPlayer>? Players { get; } = players;
	public StatsApiGame? Game { get; } = game;
}
