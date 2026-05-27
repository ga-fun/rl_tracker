namespace RlStatsApi;

internal sealed class PayloadMatchUnpaused(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
