namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadMatchEnded(
	string?	matchGuid,
	int? winnerTeamNum) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
	public int? WinnerTeamNum { get; } = winnerTeamNum;
}
