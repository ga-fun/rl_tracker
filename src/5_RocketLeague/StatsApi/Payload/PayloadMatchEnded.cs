namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadMatchEnded(
	string matchGuid,
	int winnerTeamNum) : Payload(matchGuid)
{
	public int WinnerTeamNum { get; } = winnerTeamNum;
}
