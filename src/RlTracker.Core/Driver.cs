using RlStatsApi;
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

	private readonly ConnectionManager _connectionManager;
	private readonly StatsEventHandler _eventHandler = new();
	private readonly SemaphoreSlim _gate = new(1, 1);

	private Driver()
	{
		Console.WriteLine($"{Log.Blue}[RlTracker.Core.Driver()]{Log.Reset}");
		Console.WriteLine("Loading...");
		_connectionManager = new();
		Config = Config.Load();
		RlNotFound = Config.EpicRlDir == null && Config.SteamRlDir == null;
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

		RlNotFound = newConfig.EpicRlDir == null && newConfig.SteamRlDir == null;
		if (RlNotFound)
		{
			await _connectionManager.StopAsync();
			RlNeedRestart = false;
		}
		else
		{
			bool portChanged = newConfig.StatsApiConfig.Port != Config.StatsApiConfig.Port;
			bool psrChanged = newConfig.StatsApiConfig.PacketSendRate != Config.StatsApiConfig.PacketSendRate;
			bool epicDirChanged = newConfig.EpicRlDir != Config.EpicRlDir;
			bool steamDirChanged = newConfig.SteamRlDir != Config.SteamRlDir;

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

		_connectionManager.StartAsync(
			Config.StatsApiConfig.Port,
			null,
			OnMessage,
			OnDisconnect
		);
	}
	
	private void OnMessage(string message)
	{
		try
		{
			Event apiEvent = new(message);
			_eventHandler.HandleEvent(apiEvent);
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
	}
}
