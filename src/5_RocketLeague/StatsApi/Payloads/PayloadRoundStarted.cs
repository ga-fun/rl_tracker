namespace RlStatsApi;

public sealed class PayloadRoundStarted(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
