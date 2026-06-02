namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchPaused(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
