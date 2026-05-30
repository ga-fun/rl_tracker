namespace RlStatsApi;

public sealed class PayloadGoalReplayStart(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
