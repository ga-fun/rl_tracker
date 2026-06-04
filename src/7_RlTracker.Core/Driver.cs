using System.Net;
using System.ComponentModel;
using GuillaumeAst.Utils;
using GuillaumeAst.Network;
using GuillaumeAst.RlTracker.Settings;
using GuillaumeAst.RocketLeague;
using statsApiMessageFramer = GuillaumeAst.RocketLeague.StatsApi.ApiMessageFramer;
using StatsApiEvent = GuillaumeAst.RocketLeague.StatsApi.Event;

using System.Text;	// TODO: tmp debug

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

	public static Driver Instance { get; } = new();
	public static State State { get; } = new();
	private static readonly statsApiMessageFramer ApiMessageFramer = new();
	private static readonly ApiEnventHandler ApiEnventHandler = new(State);
	private static readonly SemaphoreSlim _gate = new(1, 1);
	public Connection Connection
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
	
	private Driver()
	{
		Config = Config.Load();
		RlNotFound = RlIsNotFound(Config);
		Config.Apply(out bool rlNeedRestart);
		Connection = CreateConnection(Config.StatsApiConfig.Port);
		RlNeedRestart = rlNeedRestart;
	}

	private Connection CreateConnection(int port)
	{
		Connection connection = new(Connection.ClientType.TCP, IPAddress.Loopback, port, OnException);
		connection.BytesReceived += OnBytesReceived;
		connection.PropertyChanged += OnConnectionChanged;
		return connection;
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
		Log.Write(Log.Level.Info, "Updating core config...");

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
			if (portChanged)
			{
				await Connection.StopAsync();
				Connection = CreateConnection(newConfig.StatsApiConfig.Port);
				await Connection.StartAsync();
			}
		}
		Config = newConfig;
		Config.Save();
		Log.Write(Log.Level.Info, "Core config updated");
		await UnsafeStart();
	}

	private async Task UnsafeStart()
	{
		if (!RlNotFound)
		{
			await Connection.StartAsync();
		}
	}

	private static Connection.ExceptionAction OnException(Exception exception)
	{
		Log.Write(Log.Level.Error, $"Connection exception: {exception.GetType().Name}: {exception.Message}");
		// TODO: Stop connection on some fatal errors?
		return Connection.ExceptionAction.Continue;
	}

	private void OnBytesReceived(byte[] bytes)
	{
		try
		{
			foreach (string message in ApiMessageFramer.GetApiMessages(bytes))
			{
				StatsApiEvent apiEvent = new(message);
				ApiEnventHandler.HandleEvent(apiEvent);
			}
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
		Log.Write(Log.Level.Info, $"{Connection.Status}");
		if (Connection.Status == Connection.ConnectionStatus.Reconnecting && RlNeedRestart)
		{
			RlNeedRestart = false;
		}
	}
}
