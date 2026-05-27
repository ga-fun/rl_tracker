namespace RlStatsApi;

internal sealed class PayloadRoundStarted(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
