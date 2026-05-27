namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiBallHitBall(
	double? preHitSpeed,
	double? postHitSpeed,
	StatsApiVector? location
)
{
	public double? PreHitSpeed { get; } = preHitSpeed;
	public double? PostHitSpeed { get; } = postHitSpeed;
	public StatsApiVector? Location { get; } = location;
}
