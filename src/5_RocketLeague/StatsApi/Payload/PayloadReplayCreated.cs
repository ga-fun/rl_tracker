namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadReplayCreated(string? matchGuid) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
}
