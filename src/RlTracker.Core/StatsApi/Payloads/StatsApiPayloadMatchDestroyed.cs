namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadMatchDestroyed(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
