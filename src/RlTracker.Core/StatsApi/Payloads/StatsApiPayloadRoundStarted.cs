namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadRoundStarted(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
