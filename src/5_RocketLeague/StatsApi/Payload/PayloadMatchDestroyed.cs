namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchDestroyed(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
