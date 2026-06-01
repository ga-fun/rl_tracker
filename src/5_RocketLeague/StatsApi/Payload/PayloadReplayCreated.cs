namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadReplayCreated(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
