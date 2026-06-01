using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using GuillaumeAst.Utils;
using GuillaumeAst.RocketLeague;
using StatsApiConfig = GuillaumeAst.RocketLeague.StatsApi.Config;

namespace GuillaumeAst.RlTracker.Settings;

public sealed partial class Config : Notifier
{
	[JsonConstructor]
	private Config(){}
	private const double ApiSendPacketRateDefault = 30;
	private const string ConfigRelativeDir = "RlTracker";
	private const string ConfigFileName = "settings.json";
	private static readonly JsonSerializerOptions JsonOptions = new(){ WriteIndented = true };
	private static readonly string ConfigFile = GetConfigFile();

	private static string GetConfigFile()
	{
		string? configRootDir = null;
		try
		{
			configRootDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		}
		catch (PlatformNotSupportedException)
		{}
		if (string.IsNullOrWhiteSpace(configRootDir))
		{
			try
			{
				configRootDir = Environment.GetEnvironmentVariable("HOME");
			}
			catch (System.Security.SecurityException)
			{}
		}
		if (string.IsNullOrWhiteSpace(configRootDir))
		{
			try
			{
				configRootDir = Environment.GetEnvironmentVariable("USERPROFILE");
			}
			catch (System.Security.SecurityException)
			{}
		}
		if (string.IsNullOrWhiteSpace(configRootDir))
		{
			return Path.Combine(AppContext.BaseDirectory, ConfigFileName);
		}
		return Path.Combine(configRootDir, ConfigRelativeDir, ConfigFileName);
	}

	public ConfigUI ConfigUI { get; init; } = new();

	public StatsApiConfig StatsApiConfig { get; init; } = new(null, ApiSendPacketRateDefault);

	public InstallEpic EpicInstall { get; init; } = new();
	
	public InstallSteam SteamInstall { get; init; } = new();

	// TODO: listen for EpicInstall and SteamInstall
	// If IsValid has changed to true => Save
	public static Config Load()
	{
		Log.Print($"Loading config from: {Log.Blue}\"{ConfigFile}\".");
		if (!File.Exists(ConfigFile))
		{
			Log.PrintYellow($"Config file not found.");
			return CreateDefault();
		}
		try
		{
			string json = File.ReadAllText(ConfigFile);
			Log.Print($"Config read:\n{json}");
			Config? configMaybe = JsonSerializer.Deserialize<Config>(json, JsonOptions);
			if (configMaybe != null)
			{
				configMaybe.SubscribeInstallChanges();
				
				Log.Dump(configMaybe, $"{Log.Green}Config loaded:");
				return configMaybe;
			}
			Log.PrintRed("Unable to parse config.");
			return CreateDefault();
		}
		catch (Exception exception) when (exception
			is IOException
			or UnauthorizedAccessException
			or System.Security.SecurityException
			or JsonException)
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
		Log.Dump(this, "Saving config:");
		string? directory = Path.GetDirectoryName(ConfigFile);

		if (!string.IsNullOrWhiteSpace(directory))
		{
			Directory.CreateDirectory(directory);
		}
		string json = JsonSerializer.Serialize(this, JsonOptions);
		Log.Print("Config serialized:");
		Console.WriteLine(json);
		File.WriteAllText(ConfigFile, json);
		Log.PrintGreen($"Config saved to: {Log.Blue}\"{ConfigFile}\"{Log.Reset}.");
	}

	private static Config CreateDefault()
	{
		Log.Print("Creating default config...");
		Config config = new();
		config.SubscribeInstallChanges();
		config.EpicInstall.AutoDetectInstallDir();
		config.SteamInstall.AutoDetectInstallDir();
		config.Save();
		return config;
	}

	private void SubscribeInstallChanges()
	{
		EpicInstall.PropertyChanged += OnInstallChanged;
		SteamInstall.PropertyChanged += OnInstallChanged;
	}

	private void OnInstallChanged(object? sender, PropertyChangedEventArgs eventArgs)
	{
		if (eventArgs.PropertyName != nameof(Install.IsValid))
		{
			return;
		}
		if (sender is Install install && install.IsValid)
		{
			Save();
		}
	}
}
