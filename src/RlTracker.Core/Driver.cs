using RlStatsApi;
using RlTracker.Core.Models;

namespace RlTracker.Core;

public sealed partial class Driver
{
	public static Driver Instance { get; } = new();
	public Config Config { get; private set; }
	public bool RlNotFound { get; private set; } = false;
	public bool RlNeedRestart {
		get;
		private set
		{
			field = value && RlProcess.IsRunning();
		}
	} = false;
	public State State { get; set; } = new(State.ConnectionStatus.Disconnected);

	private readonly ConnectionManager _connectionManager;
	private readonly MessageHandler _messageHandler = new();
	private readonly SemaphoreSlim _gate = new(1, 1);

	private Driver()
	{
		Console.WriteLine($"{Log.Blue}[RlTracker.Core.Driver()]{Log.Reset}");
		Console.WriteLine("Loading...");
		_connectionManager = new();
		Config = Config.Load();
		RlNotFound = RlIsNotFound(Config);
		Config.Apply(out bool rlNeedRestart);
		RlNeedRestart = rlNeedRestart;
		Console.WriteLine($"{Log.Green}Loaded.{Log.Reset}");
	}

	public async Task Start()
	{
		await _gate.WaitAsync();
		try
		{
			UnsafeStart();
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
			await _connectionManager.StopAsync();
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
		Console.WriteLine($"{Log.Blue}[RlTracker.Core.UpdateConfig()]{Log.Reset}");
		Console.WriteLine("Updating core config...");

		RlNotFound = RlIsNotFound(newConfig);
		if (RlNotFound)
		{
			await _connectionManager.StopAsync();
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
		Console.WriteLine($"{Log.Green}Core config updated.{Log.Reset}");
		UnsafeStart();
	}

	private void UnsafeStart()
	{
		if (RlNotFound)
			return;

		State.ClientStatus = State.ConnectionStatus.Connecting;
		_connectionManager.StartAsync(
			Config.StatsApiConfig.Port,
			OnConnect,
			OnMessage,
			OnDisconnect
		);
	}
	
	private void OnConnect()
	{
		State.ClientStatus = State.ConnectionStatus.Connected;
	}

	private void OnMessage(string message)
	{
		try
		{
			Event apiEvent = new(message);
			_messageHandler.HandleEvent(apiEvent);
		}
		catch (Exception exception)
		{
			Console.WriteLine($"{Log.Red}Message parsing error: {exception.Message}.{Log.Reset}");
		}
	}

	private void OnDisconnect()
	{
		if (RlNeedRestart && !RlProcess.IsRunning())
			RlNeedRestart = false;
		State.ClientStatus = State.ConnectionStatus.Disconnected;
	}

	private static bool RlIsNotFound(Config config)
	{
		return !config.EpicInstall.IsValid && !config.SteamInstall.IsValid;
	}
}
