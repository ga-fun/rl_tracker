namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadPodiumStart(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
