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
	private const string ConfigFileName = "settings.json";
	private static readonly JsonSerializerOptions JsonOptions = new(){ WriteIndented = true };
	private static readonly string ConfigFile = Path.Combine(App.AppDir, ConfigFileName);

	public ConfigUI ConfigUI { get; init; } = new();
	public StatsApiConfig StatsApiConfig { get; init; } = new(null, ApiSendPacketRateDefault);
	public InstallEpic EpicInstall { get; init; } = new();
	public InstallSteam SteamInstall { get; init; } = new();

	private static bool IsLoading = false;

	public static Config Load()
	{
		IsLoading = true;
		Log.PrintYellow($"Loading config from: {Log.Blue}\"{ConfigFile}\"{Log.Yellow}");
		try
		{
			if (!File.Exists(ConfigFile))
			{
				Log.PrintRed($"Config file not found");
				return CreateDefault();
			}
			string json = File.ReadAllText(ConfigFile);
			Config? configMaybe = JsonSerializer.Deserialize<Config>(json, JsonOptions);
			if (configMaybe != null)
			{
				configMaybe.SubscribeInstallChanges();
				if (!configMaybe.EpicInstall.IsValid)
				{
					configMaybe.EpicInstall.AutoDetectInstallDir();
				}
				if (!configMaybe.SteamInstall.IsValid)
				{
					configMaybe.SteamInstall.AutoDetectInstallDir();
				}
				Log.Dump(configMaybe, $"{Log.Green}Config loaded:");
				return configMaybe;
			}
			Log.PrintRed("Unable to parse config");
			return CreateDefault();
		}
		catch (Exception exception) when (exception
			is IOException
			or UnauthorizedAccessException
			or System.Security.SecurityException
			or JsonException)
		{
			Log.PrintRed($"Config loading failed: {exception.Message}");
			return CreateDefault();
		}
		finally
		{
			IsLoading = false;
		}
	}

	public void Apply(out bool rlNeedRestart)
	{
		rlNeedRestart = false;
		if (EpicInstall.InstallDir != null && EpicInstall.IsValid)
		{
			StatsApiConfig.Apply(EpicInstall.InstallDir, ref rlNeedRestart);
			Log.PrintGreen("Epic config applied");
		}
		if (SteamInstall.InstallDir != null && SteamInstall.IsValid)
		{
			StatsApiConfig.Apply(SteamInstall.InstallDir, ref rlNeedRestart);
			Log.PrintGreen("Steam config applied");
		}
	}

	public void Save()
	{
		Log.PrintYellow("Saving config...");
		string? directory = Path.GetDirectoryName(ConfigFile);

		if (!string.IsNullOrWhiteSpace(directory))
		{
			Directory.CreateDirectory(directory);
		}
		string json = JsonSerializer.Serialize(this, JsonOptions);
		File.WriteAllText(ConfigFile, json);
		Log.PrintGreen($"Config saved to: {Log.Blue}\"{ConfigFile}\"{Log.Reset}");
	}

	private static Config CreateDefault()
	{
		Log.PrintYellow("Creating default config...");
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
		if (IsLoading || eventArgs.PropertyName != nameof(Install.IsValid))
		{
			return;
		}
		if (sender is Install install && install.IsValid)
		{
			Save();
		}
	}
}
