namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchPaused(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
