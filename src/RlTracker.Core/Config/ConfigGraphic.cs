namespace RlTracker.Core.Config;

internal sealed class ConfigGraphic
{
	public string? WinPrefix { get; set; } = @"✅";
	public string? LossPrefix { get; set; } = @"❌";
	public string? WinStreakPrefix { get; set; } = @"🚀";
	public string? LossStreakPrefix { get; set; } = @"⚰️";
}
