namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadMatchInitialized(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
