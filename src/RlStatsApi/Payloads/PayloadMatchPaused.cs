namespace RlStatsApi;

public sealed class PayloadMatchPaused(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
