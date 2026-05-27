namespace RlStatsApi;

internal sealed class PayloadMatchDestroyed(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
