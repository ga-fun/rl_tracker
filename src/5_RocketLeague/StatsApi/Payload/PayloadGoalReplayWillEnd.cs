namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadReplayWillEnd(string matchGuid) : IPayload
{
	public string MatchGuid { get; } = matchGuid;
}
