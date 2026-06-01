namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchInitialized(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
