namespace RlStatsApi;

public sealed class PayloadBallHit(
	string? matchGuid,
	List<PlayerRef>? players,
	BallHitBall? ball
) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
	public List<PlayerRef>? Players { get; } = players;
	public BallHitBall? Ball { get; } = ball;
}
