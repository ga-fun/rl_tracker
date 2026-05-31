namespace RlStatsApi;

public sealed class PayloadMatchDestroyed(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
