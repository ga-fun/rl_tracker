namespace RlStatsApi;

public sealed class PayloadMatchCreated(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
