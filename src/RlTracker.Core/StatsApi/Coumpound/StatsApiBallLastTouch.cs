namespace RlTracker.Core.StatsApi;

internal sealed class StatsApiBallLastTouch(StatsApiPlayerRef? player, double? speed)
{
	public StatsApiPlayerRef? Player { get; } = player;
	public double? Speed { get; } = speed;
}
