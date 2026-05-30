namespace RlStatsApi;

public sealed class PayloadMatchUnpaused(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
