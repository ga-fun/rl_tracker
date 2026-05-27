namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadMatchPaused(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
