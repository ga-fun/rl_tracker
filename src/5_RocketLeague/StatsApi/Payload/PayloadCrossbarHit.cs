namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadCrossbarHit(
	string? matchGuid,
	Vector? ballLocation,
	double? ballSpeed,
	double? impactForce,
	BallLastTouch? ballLastTouch
) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
	public Vector? BallLocation { get; } = ballLocation;
	public double? BallSpeed { get; } = ballSpeed;
	public double? ImpactForce { get; } = impactForce;
	public BallLastTouch? BallLastTouch { get; } = ballLastTouch;
}
