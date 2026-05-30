using System.Text.Json;

namespace RlTracker.Core;

public sealed partial class Config
{
	private const double ApiSendPacketRateDefault = 30;
	private static readonly JsonSerializerOptions JsonOptions = new(){ WriteIndented = true };
	private static readonly string ConfigFile = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
		"RlTracker",
		"settings.json");
	private static readonly string ProgramFiles = Environment.GetFolderPath(
		Environment.SpecialFolder.ProgramFiles);
	private static readonly string ProgramFilesX86 = Environment.GetFolderPath(
		Environment.SpecialFolder.ProgramFilesX86);
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

	public string? EpicRlDir { get; set; } = null;
	public string? SteamRlDir { get; set; } = null;

	public static Config Load()
	{
		Console.WriteLine($"{Log.Blue}[RlTracker.Core.Config.Load()]{Log.Reset}");
		Console.WriteLine($"Core config file location: {Log.Yellow}{ConfigFile}{Log.Reset}");
		Console.WriteLine("Loading...");
		if (!File.Exists(ConfigFile))
		{
			Console.WriteLine($"{Log.Yellow}Core config file not found at \"{ConfigFile}\".{Log.Reset}");
			return CreateDefault();
		}
		try
		{
			string json = File.ReadAllText(ConfigFile);
			Config? configMaybe = JsonSerializer.Deserialize<Config>(json, JsonOptions);
			if (configMaybe != null)
			{
				Console.WriteLine($"{Log.Green}Core config parsed:{Log.Reset}");
				Log.Dump(configMaybe);
				return configMaybe;
			}
			else
			{
				Console.WriteLine($"{Log.Red}Core config not parsed (null).{Log.Reset}");
				return CreateDefault();
			}
		}
		catch (Exception exception)
		{
			Console.WriteLine($"{Log.Red}Core config parsing error: {exception.Message}.{Log.Reset}");
			return CreateDefault();
		}
	}

	public void Apply(out bool rlNeedRestart)
	{
		Console.WriteLine($"{Log.Blue}[RlTracker.Core.Config.Apply()]{Log.Reset}");
		rlNeedRestart = false;
		if (EpicRlDir != null)
		{
			StatsApiConfig.Apply(EpicRlDir, ref rlNeedRestart);
			Console.WriteLine($"{Log.Green}Epic config applied{Log.Reset}.");
		}
		if (SteamRlDir != null)
		{
			StatsApiConfig.Apply(SteamRlDir, ref rlNeedRestart);
			Console.WriteLine($"{Log.Green}Steam config applied{Log.Reset}.");
		}
	}

	public void Save()
	{
		Console.WriteLine($"{Log.Blue}[RlTracker.Core.Config.Save()]{Log.Reset}");
		string? directory = Path.GetDirectoryName(ConfigFile);

		if (!string.IsNullOrWhiteSpace(directory))
		{
			Console.WriteLine($"Creating dir: \"{Log.Yellow}{directory}{Log.Reset}\"...");
			Directory.CreateDirectory(directory);
		}
		string json = JsonSerializer.Serialize(this, JsonOptions);
		Console.WriteLine("Serialized data:");
		Console.WriteLine(json);
		File.WriteAllText(ConfigFile, json);
		Console.WriteLine($"{Log.Green}Saved.{Log.Reset}");
	}

	private static Config CreateDefault()
	{
		Console.WriteLine($"{Log.Blue}[RlTracker.Core.Config.CreateDefault()]{Log.Reset}");
		Config config = new()
		{
			EpicRlDir = FindEpicRlDir(),
			SteamRlDir = FindSteamRlDir()
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
