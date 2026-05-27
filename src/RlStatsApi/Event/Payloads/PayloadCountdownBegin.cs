namespace RlStatsApi;

internal sealed class PayloadCountdownBegin(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
