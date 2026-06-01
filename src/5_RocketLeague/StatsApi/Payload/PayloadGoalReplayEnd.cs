namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadGoalReplayEnd(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
