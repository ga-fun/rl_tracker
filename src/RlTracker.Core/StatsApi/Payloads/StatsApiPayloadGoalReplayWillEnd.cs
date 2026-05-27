namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadGoalReplayWillEnd(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
