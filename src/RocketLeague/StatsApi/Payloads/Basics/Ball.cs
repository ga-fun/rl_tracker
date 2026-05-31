namespace RlStatsApi;

public sealed class Ball(
	double? speed,
	int? teamNum
)
{
	public double? Speed { get; } = speed;
	public int? TeamNum { get; } = teamNum;
}
