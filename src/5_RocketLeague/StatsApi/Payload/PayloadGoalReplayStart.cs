namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadReplayPlaybackStart(string matchGuid) : IPayload
{
	public string MatchGuid { get; } = matchGuid;
}
