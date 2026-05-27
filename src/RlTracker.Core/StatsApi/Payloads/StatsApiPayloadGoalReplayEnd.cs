namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadGoalReplayEnd(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
