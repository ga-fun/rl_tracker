namespace RlStatsApi;

internal sealed class BallHitBall(
	double? preHitSpeed,
	double? postHitSpeed,
	Vector? location
)
{
	public double? PreHitSpeed { get; } = preHitSpeed;
	public double? PostHitSpeed { get; } = postHitSpeed;
	public Vector? Location { get; } = location;
}
