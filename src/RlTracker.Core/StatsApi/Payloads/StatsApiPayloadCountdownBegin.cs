namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadCountdownBegin(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
