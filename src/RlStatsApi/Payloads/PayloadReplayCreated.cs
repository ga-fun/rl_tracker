namespace RlStatsApi;

internal sealed class PayloadReplayCreated(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
