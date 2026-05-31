namespace RlStatsApi;

public sealed class PayloadPodiumStart(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
