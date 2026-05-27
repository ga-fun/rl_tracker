namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadMatchUnpaused(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
