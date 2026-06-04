using System.ComponentModel;
using System.Windows;
using GuillaumeAst.RlTracker.Core;
using GuillaumeAst.RlTracker.Settings;
using GuillaumeAst.Utils;

namespace GuillaumeAst.RlTracker.Ui.ViewModels;

public sealed class MainWindowViewModel : Notifier
{
	private const string Orange = "#FFA500";
	private const string Red = "#D50000";
	private readonly Driver _driver;

	public Visibility MainTrackerVisibility =>
		_driver.RlNotFound || _driver.RlNeedRestart
			? Visibility.Collapsed
			: Visibility.Visible;

	public Visibility BlockingMessageVisibility =>
		_driver.RlNotFound || _driver.RlNeedRestart
			? Visibility.Visible
			: Visibility.Collapsed;

	public Visibility OpenConfigButtonVisibility =>
		_driver.RlNotFound
			? Visibility.Visible
			: Visibility.Collapsed;

	public string BlockingMessage
	{
		get
		{
			if (_driver.RlNotFound)
			{
				return ":(\n\n"
					+ "ROCKET LEAGUE NOT FOUND.\n\n"
					+ "To fix it:\n"
					+ "1. Click \"Open config file\".\n"
					+ "2. Update the Rocket League InstallDir value under EpicInstall and/or SteamInstall.\n"
					+ "3. Save the file.\n"
					+ "4. Restart RlTracker.";
			}
			else if (_driver.RlNeedRestart)
			{
				return "Rocket League config has been updated.\nPlease restart your game.";
			}
			return string.Empty;
		}
	}

	public string BlockingMessageColor =>
		_driver.RlNotFound ? Red : Orange;

	public MainWindowViewModel()
	{
		_driver = Driver.Instance;
		_driver.PropertyChanged += OnDriverChanged;
	}

    	private void OnDriverChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
        string? property = eventArgs.PropertyName;
		if (property == nameof(Driver.RlNotFound) || property == nameof(Driver.RlNeedRestart))
		{
			NotifyChange(nameof(OpenConfigButtonVisibility));
			NotifyChange(nameof(BlockingMessage));
			NotifyChange(nameof(BlockingMessageColor));
			NotifyChange(nameof(BlockingMessageVisibility));
			NotifyChange(nameof(MainTrackerVisibility));
		}
	}
}
