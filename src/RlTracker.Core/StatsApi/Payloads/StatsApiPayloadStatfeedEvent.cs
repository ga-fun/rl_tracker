namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadStatfeedEvent(
	string? eventName,
	string? type,
	StatsApiPlayerRef? mainTarget,
	string? matchGuid,
	StatsApiPlayerRef? secondaryTarget
) : StatsApiPayload
{
	public string? EventName { get; } = eventName;
	public string? Type { get; } = type;
	public StatsApiPlayerRef? MainTarget { get; } = mainTarget;
	public string? MatchGuid { get; } = matchGuid;
	public StatsApiPlayerRef? SecondaryTarget { get; } = secondaryTarget;
}
