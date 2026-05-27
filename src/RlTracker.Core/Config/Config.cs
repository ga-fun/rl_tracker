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
	/* ---------- TODO: START ---------- */
	// Move to Core project
	private static readonly string EpicRlDefaultDir = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
		"Epic Games",
		"rocketleague");
	private static readonly string SteamRlDefaultDir = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
		"Steam",
		"steamapps",
		"common",
		"rocketleague");
	/* ---------- TODO: END ---------- */

	public ConfigGraphic Graphic { get; set; } = new();
	public ConfigApi Api { get; set; } = new();
}
