namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadReplayPlaybackEnd(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
