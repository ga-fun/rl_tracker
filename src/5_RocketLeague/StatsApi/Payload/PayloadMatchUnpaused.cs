namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchUnpaused(string matchGuid) : IPayload
{
	public string MatchGuid { get; } = matchGuid;
}
