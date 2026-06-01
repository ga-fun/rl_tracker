namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadRoundStarted(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
