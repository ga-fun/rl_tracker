using System.Text.Json;

namespace RlTracker.Core.Config;

internal sealed partial class Config
{
	private static readonly string ConfigFile = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
		"RlTracker",
		"settings.json");
	private static readonly JsonSerializerOptions JsonOptions = new(){ WriteIndented = true };
	private const double ApiSendPacketRateDefault = 30;
	private static readonly string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
	private static readonly string ProgramFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
	private const string GamesDirName = "Games";

	// TODO: implement as Wpf.Config
	public ConfigGraphic WpfConfig
	{
		get;
		set { field = value ?? new(); }
	} = new();

	public RlStatsApi.Config StatsApiConfig
	{
		get;
		set { field = value ?? new(null, ApiSendPacketRateDefault); }
	} = new(null, ApiSendPacketRateDefault);

	// TODO: setter with check of IsRlInstallDir() (sinon throw)
	public string? EpicRlInstallDir { get; set; } = null;

	// TODO: setter with check of IsRlInstallDir() (sinon throw)
	public string? SteamRlInstallDir { get; set; } = null;

	public static Config Load()
	{
		if (!File.Exists(ConfigFile))
			return CreateDefault();
		try
		{
			string json = File.ReadAllText(ConfigFile);
			return JsonSerializer.Deserialize<Config>(json, JsonOptions)
				?? CreateDefault();
		}
		catch
		{
			return CreateDefault();
		}
	}

	public void Apply(out bool rlNeedRestart)
	{
		if (EpicRlInstallDir == null && SteamRlInstallDir == null)
			throw new InvalidOperationException("Epic and/or Steam install dir must be set before applying config.");

		rlNeedRestart = false;
		if (EpicRlInstallDir != null)
			StatsApiConfig.Apply(EpicRlInstallDir, ref rlNeedRestart);
		if (SteamRlInstallDir != null)
			StatsApiConfig.Apply(SteamRlInstallDir, ref rlNeedRestart);
		Save();
	}

	public void Save()
	{
		string? directory = Path.GetDirectoryName(ConfigFile);

		if (!string.IsNullOrWhiteSpace(directory))
			Directory.CreateDirectory(directory);
		string json = JsonSerializer.Serialize(this, JsonOptions);
		File.WriteAllText(ConfigFile, json);
	}

	private static Config CreateDefault()
	{
		Config config = new()
		{
			EpicRlInstallDir = FindEpicRlInstallDir(),
			SteamRlInstallDir = FindSteamRlInstallDir()
		};
		config.Save();
		return config;
	}

	private static bool IsRlInstallDir(string installDir)
	{
		return Directory.Exists(installDir)
			&& File.Exists(Path.Combine(
				installDir,
				RlStatsApi.Config.ConfigFileRelativePath
			));
	}
}
