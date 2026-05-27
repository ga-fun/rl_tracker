namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadMatchCreated(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
