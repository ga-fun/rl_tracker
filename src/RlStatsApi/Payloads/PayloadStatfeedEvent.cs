namespace RlStatsApi;

internal sealed class PayloadStatfeedEvent(
	string? eventName,
	string? type,
	PlayerRef? mainTarget,
	string? matchGuid,
	PlayerRef? secondaryTarget
) : Payload
{
	public string? EventName { get; } = eventName;
	public string? Type { get; } = type;
	public PlayerRef? MainTarget { get; } = mainTarget;
	public string? MatchGuid { get; } = matchGuid;
	public PlayerRef? SecondaryTarget { get; } = secondaryTarget;
}
