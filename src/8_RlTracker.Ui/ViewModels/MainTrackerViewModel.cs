using GuillaumeAst.Utils;

namespace GuillaumeAst.RlTracker.Ui.ViewModels;

public class MainTrackerViewModel : Notifier
{
	public string WinPrefix { get; } = "✅";
	public string LossPrefix { get; } = "❌";
	public string StreakPrefix { get; } = "🚀";
	public int WinCount { get; } = 7;
	public int LossCount { get; } = 3;
	public int StreakCount { get; } = 2;
	public string Status { get; } = "Connected";
}
