namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchDestroyed(string matchGuid) : IPayload
{
	public string MatchGuid { get; } = matchGuid;
}
