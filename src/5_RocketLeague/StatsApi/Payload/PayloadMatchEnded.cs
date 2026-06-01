namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchEnded(
	string? matchGuid,
	int? winnerTeamNum) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
	public int? WinnerTeamNum { get; } = winnerTeamNum;
}
