namespace RlStatsApi;

internal sealed class PayloadPodiumStart(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
