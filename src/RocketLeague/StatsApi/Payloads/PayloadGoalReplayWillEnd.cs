namespace RlStatsApi;

public sealed class PayloadGoalReplayWillEnd(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
