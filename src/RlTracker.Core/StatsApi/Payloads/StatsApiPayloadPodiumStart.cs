namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadPodiumStart(string? matchGuid) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
