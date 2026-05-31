using System.Text.Json;

namespace RlTracker.Core;

public sealed partial class Config : Notifier
{
	// No constructor, use Config.Load() instead
	private Config(){}
	private const double ApiSendPacketRateDefault = 30;
	private static readonly JsonSerializerOptions JsonOptions = new(){ WriteIndented = true };
	private static readonly string ConfigFile = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
		"RlTracker",
		"settings.json");

	// TODO: placeholder (waiting for RlTracker.Ui implementation)
	public ConfigGraphic WpfConfig
	{
		get;
		set
		{
			field = value ?? new();
			NotifyChange();
		}
	} = new();

	public RlStatsApi.Config StatsApiConfig
	{
		get;
		set
		{
			field = value ?? new(null, ApiSendPacketRateDefault);
			NotifyChange();
		}
	} = new(null, ApiSendPacketRateDefault);

	public RlInstallEpic EpicInstall
	{
		get;
		set
		{
			field = value;
			NotifyChange();
		}
	} = new(null);
	
	public RlInstallSteam SteamInstall
	{
		get;
		set
		{
			field = value;
			NotifyChange();
		}
	} = new(null);

	public static Config Load()
	{
		Log.Print($"Loading config from: \"{Log.Yellow}{ConfigFile}{Log.Reset}\".");
		if (!File.Exists(ConfigFile))
		{
			Log.PrintYellow($"Config file not found.");
			return CreateDefault();
		}
		try
		{
			string json = File.ReadAllText(ConfigFile);
			Config? configMaybe = JsonSerializer.Deserialize<Config>(json, JsonOptions);
			if (configMaybe != null)
			{
				Log.Dump(configMaybe, $"{Log.Green}Config loaded:");
				return configMaybe;
			}
			else
			{
				Log.PrintRed("Unable to parse config.");
				return CreateDefault();
			}
		}
		catch (Exception exception)
		{
			Log.PrintRed($"Config loading failed: {exception.Message}.");
			return CreateDefault();
		}
	}

	public void Apply(out bool rlNeedRestart)
	{
		rlNeedRestart = false;
		if (EpicInstall.InstallDir != null && EpicInstall.IsValid)
		{
			StatsApiConfig.Apply(EpicInstall.InstallDir, ref rlNeedRestart);
			Log.PrintGreen("Epic config applied.");
		}
		if (SteamInstall.InstallDir != null && SteamInstall.IsValid)
		{
			StatsApiConfig.Apply(SteamInstall.InstallDir, ref rlNeedRestart);
			Log.PrintGreen("Steam config applied.");
		}
	}

	public void Save()
	{
		string? directory = Path.GetDirectoryName(ConfigFile);

		if (!string.IsNullOrWhiteSpace(directory))
			Directory.CreateDirectory(directory);
		string json = JsonSerializer.Serialize(this, JsonOptions);
		Console.WriteLine(json);
		File.WriteAllText(ConfigFile, json);
		Log.PrintGreen($"Config saved to: \"{ConfigFile}\"");
	}

	private static Config CreateDefault()
	{
		Log.Print("Creating default config...");
		Config config = new();
		config.Save();
		return config;
	}
}
