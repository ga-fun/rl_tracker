namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadCountdownBegin(string matchGuid) : IPayload
{
	public string MatchGuid { get; } = matchGuid;
}
