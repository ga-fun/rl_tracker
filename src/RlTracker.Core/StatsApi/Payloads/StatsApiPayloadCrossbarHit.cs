namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadCrossbarHit(
	string? matchGuid,
	StatsApiVector? ballLocation,
	double? ballSpeed,
	double? impactForce,
	StatsApiBallLastTouch? ballLastTouch
) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
	public StatsApiVector? BallLocation { get; } = ballLocation;
	public double? BallSpeed { get; } = ballSpeed;
	public double? ImpactForce { get; } = impactForce;
	public StatsApiBallLastTouch? BallLastTouch { get; } = ballLastTouch;
}
