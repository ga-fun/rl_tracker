namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadRoundStarted(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
