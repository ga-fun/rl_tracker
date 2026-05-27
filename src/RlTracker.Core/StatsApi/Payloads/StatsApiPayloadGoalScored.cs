namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiPayloadGoalScored(
	string? matchGuid,
	double? goalSpeed,
	double? goalTime,
	StatsApiVector? impactLocation,
	StatsApiPlayerRef? scorer,
	StatsApiPlayerRef? assister,
	StatsApiBallLastTouch? ballLastTouch
) : StatsApiPayload
{
	public string? MatchGuid { get; } = matchGuid;
	public double? GoalSpeed { get; } = goalSpeed;
	public double? GoalTime { get; } = goalTime;
	public StatsApiVector? ImpactLocation { get; } = impactLocation;
	public StatsApiPlayerRef? Scorer { get; } = scorer;
	public StatsApiPlayerRef? Assister { get; } = assister;
	public StatsApiBallLastTouch? BallLastTouch { get; } = ballLastTouch;
}
