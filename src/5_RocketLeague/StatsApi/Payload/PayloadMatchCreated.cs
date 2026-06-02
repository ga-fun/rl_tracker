namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchCreated(string? matchGuid) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
}
