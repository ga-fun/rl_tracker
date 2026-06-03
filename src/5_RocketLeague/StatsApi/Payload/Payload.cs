namespace GuillaumeAst.RocketLeague.StatsApi;

public abstract class Payload(string matchGuid)
{
    public string MatchGuid { get; } = matchGuid;
}
