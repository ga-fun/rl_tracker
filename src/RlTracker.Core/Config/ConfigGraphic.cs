namespace RlTracker.Core.Config;

internal sealed class ConfigGraphic
{
	private const string WinPrefixDefault = "✅";
	private const string LossPrefixDefault = "❌";
	private const string WinStreakPrefixDefault = "🚀";
	private const string LossStreakPrefixDefault = "⚰️";

	public string WinPrefix
	{
		get;
		set { field = value ?? WinPrefixDefault; }
	} = WinPrefixDefault;

	public string LossPrefix
	{
		get;
		set { field = value ?? LossPrefixDefault; }
	} = LossPrefixDefault;

	public string WinStreakPrefix
	{
		get;
		set { field = value ?? WinStreakPrefixDefault; }
	} = WinStreakPrefixDefault;

	public string LossStreakPrefix
	{
		get;
		set { field = value ?? LossStreakPrefixDefault; }
	} = LossStreakPrefixDefault;
}
