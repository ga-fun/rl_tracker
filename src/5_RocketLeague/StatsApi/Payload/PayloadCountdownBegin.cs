namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadCountdownBegin(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
