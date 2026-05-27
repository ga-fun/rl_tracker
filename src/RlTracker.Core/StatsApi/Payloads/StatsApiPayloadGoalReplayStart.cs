namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadGoalReplayStart(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
