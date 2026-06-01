using System.ComponentModel;
using GuillaumeAst.Utils;
using GuillaumeAst.Network;
using GuillaumeAst.RocketLeague;
using StatsApiEvent = GuillaumeAst.RocketLeague.StatsApi.Event;
using GuillaumeAst.RlTracker.Settings;
using GuillaumeAst.RlTracker.Core.Models;

namespace GuillaumeAst.RlTracker.Core;

public sealed partial class Driver : Notifier
{
	public static Driver Instance { get; } = new();
	public Config Config
	{
		get;
		private set
		{
			if (field != value)
			{
				field = value;
				NotifyChange();
			}
		}
	}
	public bool RlNotFound
	{
		get;
		private set
		{
			if (field != value)
			{
				field = value;
				NotifyChange();
			}
		}
	} = false;
	public bool RlNeedRestart {
		get;
		private set
		{
			bool newValue = value && RlProcess.IsRunning();
			if (field != newValue)
			{
				field = newValue;
				NotifyChange();
			}
		}
	} = false;

	public State State { get; } = new();
	public Connection Connection { get; }
	private readonly MessageHandler _messageHandler = new();
	private readonly SemaphoreSlim _gate = new(1, 1);

	private Driver()
	{
		Log.Print("Loading...");
		Connection = new();
		Connection.MessageReceived += OnMessage;
		Connection.PropertyChanged += OnConnectionChanged;
		Config = Config.Load();
		RlNotFound = RlIsNotFound(Config);
		Config.Apply(out bool rlNeedRestart);
		RlNeedRestart = rlNeedRestart;
		Log.PrintGreen("Loaded.");
	}

	public async Task Start()
	{
		await _gate.WaitAsync();
		try
		{
			await UnsafeStart();
		}
		finally
		{
			_gate.Release();
		}
	}

	public async Task Stop()
	{
		await _gate.WaitAsync();
		try
		{
			await Connection.StopAsync();
		}
		finally
		{
			_gate.Release();
		}
	}

	public async Task UpdateConfig(Config newConfig)
	{
		ArgumentNullException.ThrowIfNull(newConfig);

		await _gate.WaitAsync();
		try
		{
			await UnsafeUpdateConfigAsync(newConfig);
		}
		finally
		{
			_gate.Release();
		}
	}

	private async Task UnsafeUpdateConfigAsync(Config newConfig)
	{
		Log.Print("Updating core config...");

		RlNotFound = RlIsNotFound(newConfig);
		if (RlNotFound)
		{
			await Connection.StopAsync();
			RlNeedRestart = false;
		}
		else
		{
			bool portChanged = newConfig.StatsApiConfig.Port != Config.StatsApiConfig.Port;
			bool psrChanged = newConfig.StatsApiConfig.PacketSendRate != Config.StatsApiConfig.PacketSendRate;
			bool epicDirChanged = newConfig.EpicInstall.InstallDir != Config.EpicInstall.InstallDir;
			bool steamDirChanged = newConfig.SteamInstall.InstallDir != Config.SteamInstall.InstallDir;

			if (epicDirChanged || steamDirChanged || portChanged || psrChanged)
			{
				newConfig.Apply(out bool rlNeedRestart);
				RlNeedRestart = rlNeedRestart;
			}
		}
		Config = newConfig;
		Config.Save();
		Log.PrintGreen("Core config updated.");
		await UnsafeStart();
	}

	private async Task UnsafeStart()
	{
		if (!RlNotFound)
		{
			await Connection.StartAsync(Config.StatsApiConfig.Port, OnException);
		}
	}

	private Connection.ExceptionAction OnException(Exception exception)
	{
		Log.PrintRed($"Connection exception: {exception.GetType().Name}: {exception.Message}.");
		// TODO
		return Connection.ExceptionAction.Continue;
	}

	private void OnMessage(string message)
	{
		try
		{
			StatsApiEvent apiEvent = new(message);
			_messageHandler.HandleEvent(apiEvent);
		}
		catch (Exception exception) when (exception
			is FormatException
			or NotSupportedException)
		{
			Log.PrintRed($"Message parsing error: {exception.GetType().Name}: {exception.Message}.");
		}
	}

	private void OnConnectionChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		if (eventArgs.PropertyName != nameof(Connection.Status))
		{
			return;
		}
		if (Connection.Status == Connection.ConnectionStatus.Disconnected)
		{
			OnDisconnect();
		}
	}

	private void OnDisconnect()
	{
		if (RlNeedRestart && !RlProcess.IsRunning())
		{
			RlNeedRestart = false;
		}
	}

	private static bool RlIsNotFound(Config config)
	{
		return !config.EpicInstall.IsValid && !config.SteamInstall.IsValid;
	}
}
