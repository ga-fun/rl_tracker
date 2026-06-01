namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadGoalReplayWillEnd(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
