namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadStatfeedEvent(
	string eventName,
	string type,
	PlayerRef mainTarget,
	string matchGuid,
	PlayerRef? secondaryTarget
) : Payload(matchGuid)
{
	public string EventName { get; } = eventName;
	public string Type { get; } = type;
	public PlayerRef MainTarget { get; } = mainTarget;
	public PlayerRef? SecondaryTarget { get; } = secondaryTarget;
}
