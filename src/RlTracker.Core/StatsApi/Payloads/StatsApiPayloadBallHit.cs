namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadBallHit(
	string? matchGuid,
	List<StatsApiPlayerRef>? players,
	StatsApiBallHitBall? ball
) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
	public List<StatsApiPlayerRef>? Players { get; } = players;
	public StatsApiBallHitBall? Ball { get; } = ball;
}
