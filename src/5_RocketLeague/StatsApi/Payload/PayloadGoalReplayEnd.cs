namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadGoalReplayEnd(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
