using System.ComponentModel;
using GuillaumeAst.Utils;
using GuillaumeAst.Network;
using GuillaumeAst.RocketLeague;
using StatsApiEvent = GuillaumeAst.RocketLeague.StatsApi.Event;
using GuillaumeAst.RlTracker.Settings;

namespace GuillaumeAst.RlTracker.Core;

public sealed partial class Driver : Notifier
{
	/* ---------- TODO (START): move to RocketLeague Project ---------- */
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

	private static bool RlIsNotFound(Config config)
	{
		return !config.EpicInstall.IsValid && !config.SteamInstall.IsValid;
	}
	/* ---------- TODO (END): move to RocketLeague Project ---------- */

	public static readonly State State = new();
	public static readonly Connection Connection = new();
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
	public static Driver Instance { get; } = new();
	
	private static readonly ApiEnventHandler ApiEnventHandler = new(State);
	private static readonly SemaphoreSlim _gate = new(1, 1);

	private Driver()
	{
		Log.Write(Log.Level.Info, $"Logs will be stored in: {Log.Blue}\"{Log.LogFile}\"");
		Connection.MessageReceived += OnMessage;
		Connection.PropertyChanged += OnConnectionChanged;
		Config = Config.Load();
		RlNotFound = RlIsNotFound(Config);
		Config.Apply(out bool rlNeedRestart);
		RlNeedRestart = rlNeedRestart;
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

	public static async Task Stop()
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
		Log.Write(Log.Level.Info, $"{Log.Yellow}Updating core config...");

		RlNotFound = RlIsNotFound(newConfig);
		if (RlNotFound)
		{
			await Connection.StopAsync();
			RlNeedRestart = false;
		}
		else
		{
			bool portChanged = newConfig.StatsApiConfig.Port != Config.StatsApiConfig.Port;
			bool psrChanged = !Maths.DoublesAreEqual(newConfig.StatsApiConfig.PacketSendRate, Config.StatsApiConfig.PacketSendRate);
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
		Log.Write(Log.Level.Info, $"{Log.Green}Core config updated");
		await UnsafeStart();
	}

	private async Task UnsafeStart()
	{
		if (!RlNotFound)
		{
			await Connection.StartAsync(Config.StatsApiConfig.Port, OnException);
		}
	}

	private static Connection.ExceptionAction OnException(Exception exception)
	{
		Log.Write(Log.Level.Error, $"Connection exception: {exception.GetType().Name}: {exception.Message}");
		// TODO: Stop connection on some fatal errors?
		return Connection.ExceptionAction.Continue;
	}

	private void OnMessage(string message)
	{
		try
		{
			StatsApiEvent apiEvent = new(message);
			ApiEnventHandler.HandleEvent(apiEvent);
		}
		catch (Exception exception) when (exception
			is FormatException
			or NotSupportedException)
		{
			Log.Write(Log.Level.Error, $"Message parsing error: {exception.GetType().Name}: {exception.Message}");
		}
	}

	private void OnConnectionChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		if (eventArgs.PropertyName != nameof(Connection.Status))
		{
			return;
		}
		Connection.ConnectionStatus status = Connection.Status;

		if (status == Connection.ConnectionStatus.Connected)
		{
			Log.Write(Log.Level.Info, $"{Log.Green}{status}");
		}
		else if (status == Connection.ConnectionStatus.Disconnected)
		{
			Log.Write(Log.Level.Info, $"{Log.Red}{status}");
			OnDisconnect();
		}
		else
		{
			Log.Write(Log.Level.Info, $"{Log.Yellow}{status}...");
		}
	}

	private void OnDisconnect()
	{
		if (RlNeedRestart && !RlProcess.IsRunning())
		{
			RlNeedRestart = false;
		}
	}
}
