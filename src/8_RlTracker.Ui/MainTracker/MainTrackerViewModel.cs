using System.Windows;
using System.ComponentModel;
using GuillaumeAst.Network;
using GuillaumeAst.RlTracker.Core;
using GuillaumeAst.RlTracker.Settings;
using GuillaumeAst.Utils;

namespace GuillaumeAst.RlTracker.Ui.ViewModels;

public class MainTrackerViewModel : Notifier
{
	private const string Orange = "#FFA500";
	private const string Green = "#4CAF50";
	private const string Red = "#D50000";
	private const string Gray = "#808080";
	private const string White = "#DDDDDD";
	private readonly Driver _driver;
	private readonly State _state;
	private Tracker _tracker;
	private ConfigUI _configUI;
	private Connection _connection;

	public Visibility GameModeValueVisibility =>
		WinCount == 0 && LossCount == 0
			? Visibility.Collapsed
			: Visibility.Visible;
	public string GameModeValue => _state.CurrentGameMode switch
	{
		GameMode.OneVersusOne => "1v1",
		GameMode.TwoVersusTwo => "2v2",
		GameMode.ThreeVersusThree => "3v3",
		_ => "Custom Game Mode"
	};
	public string GameModeColor => _state.CurrentGameMode switch
	{
		GameMode.Other => "#808080",
		_ => White
	};
	public uint WinCount => _tracker.Win;
	public uint LossCount => _tracker.Loss;
	public int StreakCount => _tracker.Streak;
	public string StreakPrefix => _tracker.Streak >= 0 ? _configUI.WinStreakPrefix : _configUI.LossStreakPrefix;
	public string WinPrefix => _configUI.WinPrefix;
	public string LossPrefix => _configUI.LossPrefix;
	public Connection.ConnectionStatus ConnectionStatus => _connection.Status;
	public string ConnectionStatusColor => _connection.Status switch
	{
		Connection.ConnectionStatus.Connecting => Orange,
		Connection.ConnectionStatus.Connected => Green,
		Connection.ConnectionStatus.Reconnecting => Orange,
		Connection.ConnectionStatus.Disconnecting => Orange,
		Connection.ConnectionStatus.Disconnected => Red,
		_ => Gray
	};

	public MainTrackerViewModel()
	{
		_driver = Driver.Instance;
		_state = Driver.State;
		_tracker = _state.CurrentTracker;
		_configUI = _driver.Config.ConfigUI;
		_connection = _driver.Connection;

		_driver.PropertyChanged += OnDriverChanged;
		_state.PropertyChanged += OnStateChanged;
		_tracker.PropertyChanged += OnTrackerChanged;
		_configUI.PropertyChanged += OnConfigUiChanged;
		_connection.PropertyChanged += OnConnectionChanged;
	}

	private void OnDriverChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		if (eventArgs.PropertyName == nameof(Driver.Config))
		{
			_configUI.PropertyChanged -= OnConfigUiChanged;
			_configUI = _driver.Config.ConfigUI;
			_configUI.PropertyChanged += OnConfigUiChanged;
			NotifyChange(nameof(WinPrefix));
			NotifyChange(nameof(LossPrefix));
			NotifyChange(nameof(StreakPrefix));
		}
		else if (eventArgs.PropertyName == nameof(Driver.Connection))
		{
			_connection.PropertyChanged -= OnConnectionChanged;
			_connection = _driver.Connection;
			_connection.PropertyChanged += OnConnectionChanged;
			NotifyChange(nameof(ConnectionStatus));
			NotifyChange(nameof(ConnectionStatusColor));
		}
	}

	private void OnStateChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		if (eventArgs.PropertyName == nameof(State.CurrentGameMode))
		{
			NotifyChange(nameof(GameModeValue));
			NotifyChange(nameof(GameModeColor));
		}
		else if (eventArgs.PropertyName == nameof(State.CurrentTracker))
		{
			_tracker.PropertyChanged -= OnTrackerChanged;
			_tracker = _state.CurrentTracker;
			_tracker.PropertyChanged += OnTrackerChanged;
			NotifyChange(nameof(WinCount));
			NotifyChange(nameof(LossCount));
			NotifyChange(nameof(StreakCount));
			NotifyChange(nameof(StreakPrefix));
			NotifyChange(nameof(GameModeValueVisibility));
		}
	}

	private void OnTrackerChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		if (eventArgs.PropertyName == nameof(Tracker.Win))
		{
			NotifyChange(nameof(WinCount));
			NotifyChange(nameof(GameModeValueVisibility));
		}
		else if (eventArgs.PropertyName == nameof(Tracker.Loss))
		{
			NotifyChange(nameof(LossCount));
			NotifyChange(nameof(GameModeValueVisibility));
		}
		else if (eventArgs.PropertyName == nameof(Tracker.Streak))
		{
			NotifyChange(nameof(StreakPrefix));
			NotifyChange(nameof(StreakCount));
		}
	}

	private void OnConfigUiChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		string? prop = eventArgs.PropertyName;

		if (prop == nameof(ConfigUI.WinPrefix))
		{
			NotifyChange(nameof(WinPrefix));
		}
		else if (prop == nameof(ConfigUI.LossPrefix))
		{
			NotifyChange(nameof(LossPrefix));
		}
		else if (prop == nameof(ConfigUI.WinStreakPrefix) || prop == nameof(ConfigUI.LossStreakPrefix))
		{
			NotifyChange(nameof(StreakPrefix));
		}
	}

	private void OnConnectionChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		if (eventArgs.PropertyName == nameof(Connection.Status))
		{
			NotifyChange(nameof(ConnectionStatus));
			NotifyChange(nameof(ConnectionStatusColor));
		}
	}
}
