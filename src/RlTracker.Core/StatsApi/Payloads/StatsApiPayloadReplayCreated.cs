namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadReplayCreated(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
