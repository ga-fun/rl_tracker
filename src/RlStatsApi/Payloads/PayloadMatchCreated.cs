namespace RlStatsApi;

internal sealed class PayloadMatchCreated(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
