namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadBallHit(
	string? matchGuid,
	IReadOnlyList<PlayerRef>? players,
	BallHitBall? ball
) : IPayload
{
	public string? MatchGuid { get; } = matchGuid;
	public IReadOnlyList<PlayerRef>? Players { get; } = players;
	public BallHitBall? Ball { get; } = ball;
}
