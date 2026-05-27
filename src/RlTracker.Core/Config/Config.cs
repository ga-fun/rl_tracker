using System.Text.Json;

namespace RlTracker.Core.Config;

/*
Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

Directory.CreateDirectory(configDir);
string json = File.ReadAllText(configPath);
File.WriteAllText(configPath, json);

JsonSerializer.Serialize(config, options);
JsonSerializer.Deserialize<Config>(json, options);
*/

internal sealed class Config
{
	public ConfigGraphic Graphic { get; set; } = new();
	public ConfigApi Api { get; set; } = new();
	public ConfigIni EpicConfig { get; set; } = @"C:\Program Files\Epic Games\rocketleague";
	public ConfigIni SteamConfig { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\rocketleague";
}
