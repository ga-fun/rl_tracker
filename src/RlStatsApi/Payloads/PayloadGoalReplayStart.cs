namespace RlStatsApi;

internal sealed class PayloadGoalReplayStart(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
