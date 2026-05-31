namespace RlTracker.Core;

// TODO: move it to RlTracker.Ui
public sealed class ConfigGraphic : Notifier
{
	private const string WinPrefixDefault = "✅";
	private const string LossPrefixDefault = "❌";
	private const string WinStreakPrefixDefault = "🚀";
	private const string LossStreakPrefixDefault = "⚰️";

	public string WinPrefix
	{
		get;
		set
		{
			field = value ?? WinPrefixDefault;
			NotifyChange();
		}
	} = WinPrefixDefault;

	public string LossPrefix
	{
		get;
		set
		{
			field = value ?? LossPrefixDefault;
			NotifyChange();
		}
	} = LossPrefixDefault;

	public string WinStreakPrefix
	{
		get;
		set
		{
			field = value ?? WinStreakPrefixDefault;
			NotifyChange();
		}
	} = WinStreakPrefixDefault;

	public string LossStreakPrefix
	{
		get;
		set
		{
			field = value ?? LossStreakPrefixDefault;
			NotifyChange();
		}
	} = LossStreakPrefixDefault;
}
