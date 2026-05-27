namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiBall(
	double? speed,
	int? teamNum
)
{
	public double? Speed { get; } = speed;
	public int? TeamNum { get; } = teamNum;
}
