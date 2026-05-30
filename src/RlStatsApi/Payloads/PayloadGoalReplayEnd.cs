namespace RlStatsApi;

public sealed class PayloadGoalReplayEnd(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
