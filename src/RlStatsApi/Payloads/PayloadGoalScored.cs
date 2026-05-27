namespace RlStatsApi;

internal sealed class PayloadGoalScored(
	string? matchGuid,
	double? goalSpeed,
	double? goalTime,
	Vector? impactLocation,
	PlayerRef? scorer,
	PlayerRef? assister,
	BallLastTouch? ballLastTouch
) : Payload
{
	public string? MatchGuid { get; } = matchGuid;
	public double? GoalSpeed { get; } = goalSpeed;
	public double? GoalTime { get; } = goalTime;
	public Vector? ImpactLocation { get; } = impactLocation;
	public PlayerRef? Scorer { get; } = scorer;
	public PlayerRef? Assister { get; } = assister;
	public BallLastTouch? BallLastTouch { get; } = ballLastTouch;
}
