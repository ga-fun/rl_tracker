namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiVector(double? x, double? y, double? z)
{
	public double? X { get; } = x;
	public double? Y { get; } = y;
	public double? Z { get; } = z;
}
