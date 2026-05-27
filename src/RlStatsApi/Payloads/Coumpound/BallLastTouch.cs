namespace RlStatsApi;

internal sealed class BallLastTouch(PlayerRef? player, double? speed)
{
	public PlayerRef? Player { get; } = player;
	public double? Speed { get; } = speed;
}
