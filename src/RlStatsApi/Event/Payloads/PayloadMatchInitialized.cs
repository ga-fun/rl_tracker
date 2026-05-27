namespace RlStatsApi;

internal sealed class PayloadMatchInitialized(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
