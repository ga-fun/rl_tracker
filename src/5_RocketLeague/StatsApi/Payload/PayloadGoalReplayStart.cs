namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadGoalReplayStart(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
