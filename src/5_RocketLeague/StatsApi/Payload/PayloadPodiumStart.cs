namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadPodiumStart(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
