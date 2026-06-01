namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadGoalReplayStart(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
