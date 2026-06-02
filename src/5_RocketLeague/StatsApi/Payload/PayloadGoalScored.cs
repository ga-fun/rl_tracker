namespace GuillaumeAst.RocketLeague.StatsApi;

public sealed class PayloadGoalScored(
	string matchGuid,
	double goalSpeed,
	double goalTime,
	Vector impactLocation,
	PlayerRef scorer,
	BallLastTouch ballLastTouch,
	PlayerRef? assister
) : IPayload
{
	public string MatchGuid { get; } = matchGuid;
	public double GoalSpeed { get; } = goalSpeed;
	public double GoalTime { get; } = goalTime;
	public Vector ImpactLocation { get; } = impactLocation;
	public PlayerRef Scorer { get; } = scorer;
	public BallLastTouch BallLastTouch { get; } = ballLastTouch;
	public PlayerRef? Assister { get; } = assister;
}
