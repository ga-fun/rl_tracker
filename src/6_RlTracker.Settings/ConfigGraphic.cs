using GuillaumeAst.Utils;

namespace GuillaumeAst.RlTracker.Settings;

public sealed class ConfigUI : Notifier
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
			string normalized = value ?? WinPrefixDefault;
			if (field == normalized)
			{
				return;
			}
			field = normalized;
			NotifyChange();
		}
	} = WinPrefixDefault;

	public string LossPrefix
	{
		get;
		set
		{
			string normalized = value ?? LossPrefixDefault;
			if (field == normalized)
			{
				return;
			}
			field = normalized;
			NotifyChange();
		}
	} = LossPrefixDefault;

	public string WinStreakPrefix
	{
		get;
		set
		{
			string normalized = value ?? WinStreakPrefixDefault;
			if (field == normalized)
			{
				return;
			}
			field = normalized;
			NotifyChange();
		}
	} = WinStreakPrefixDefault;

	public string LossStreakPrefix
	{
		get;
		set
		{
			string normalized = value ?? LossStreakPrefixDefault;
			if (field == normalized)
			{
				return;
			}
			field = normalized;
			NotifyChange();
		}
	} = LossStreakPrefixDefault;
}
