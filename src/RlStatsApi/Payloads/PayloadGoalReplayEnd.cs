namespace RlStatsApi;

internal sealed class PayloadGoalReplayEnd(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
