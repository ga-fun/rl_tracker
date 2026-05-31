using System.Text.Json;

namespace RlTracker.Core;

public sealed class RlInstallEpic(string? installDir) : RlInstall(installDir)
{
	private const string EpicGamesDirName = "Epic Games";
	private const string EpicRlDirName = "rocketleague";
	private const string EpicManifestPattern = "*.item";
	private const string EpicInstallLocationProperty = "InstallLocation";
	private static readonly string ManifestsDir = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
		"Epic",
		"EpicGamesLauncher",
		"Data",
		"Manifests");

	public override void FindInstallDir()
	{
		try
		{
			if (TryFindFromManifests() || TryFindFromClassicPaths())
				Console.WriteLine($"{Log.Green}RL Epic dir found atuomatically: {Log.Yellow}{InstallDir}.{Log.Reset}");
			else
				Console.WriteLine($"{Log.Red}RL Epic dir not found atuomatically.{Log.Reset}");
		}
		catch (Exception exception)
		{
			Console.WriteLine($"{Log.Red}Exception while searching RL Epic dir: {exception.Message}.{Log.Reset}");
		}
	}

	private bool TryFindFromManifests()
	{
		if (!Directory.Exists(ManifestsDir))
			return false;
		try
		{
			foreach (string manifestFile in Directory.EnumerateFiles(ManifestsDir, EpicManifestPattern))
			{
				if (TryReadInstallDirInManifest(manifestFile, out string installDir)
					&& InstallDirIsValid(installDir))
				{
					InstallDir = installDir;
					return true;
				}
			}
		}
		catch
		{
			return false;
		}
		return false;
	}

	private static bool TryReadInstallDirInManifest(string manifestFile, out string installDir)
	{
		installDir = "";
		try
		{
			string json = File.ReadAllText(manifestFile);
			using JsonDocument document = JsonDocument.Parse(json);

			if (!document.RootElement.TryGetProperty(EpicInstallLocationProperty, out JsonElement property))
				return false;
			if (property.ValueKind != JsonValueKind.String)
				return false;
			installDir = (property.GetString() ?? "").Trim();
			return installDir.Length > 0;
		}
		catch
		{
			return false;
		}
	}

	private bool TryFindFromClassicPaths()
	{
		foreach (string epicRoot in GetRootCandidates())
		{
			string installDir = Path.Combine(epicRoot, EpicRlDirName);

			if (InstallDirIsValid(installDir))
			{
				InstallDir = installDir;
				return true;
			}
		}
		return false;
	}

	private static IEnumerable<string> GetRootCandidates()
	{
		if (!string.IsNullOrWhiteSpace(ProgramFilesDir))
			yield return Path.Combine(ProgramFilesDir, EpicGamesDirName);
		if (!string.IsNullOrWhiteSpace(ProgramFilesX86Dir))
			yield return Path.Combine(ProgramFilesX86Dir, EpicGamesDirName);
		foreach (DriveInfo drive in DriveInfo.GetDrives())
		{
			if (!drive.IsReady)
				continue;
			yield return Path.Combine(drive.RootDirectory.FullName, EpicGamesDirName);
			yield return Path.Combine(drive.RootDirectory.FullName, GamesDirName);
			yield return Path.Combine(drive.RootDirectory.FullName, GamesDirName, EpicGamesDirName);
		}
	}
}
