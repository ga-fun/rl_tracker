namespace RlStatsApi;

internal sealed class PayloadMatchPaused(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
