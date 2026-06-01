namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchInitialized(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
