namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadGoalReplayWillEnd(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
