using System.Text.Json;

namespace RlTracker.Core;

public sealed partial class Config
{
	private const string EpicGamesDirName = "Epic Games";
	private const string EpicRlDirName = "rocketleague";
	private const string EpicManifestPattern = "*.item";
	private const string EpicInstallLocationProperty = "InstallLocation";
	private static readonly string EpicManifestsDir = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
		"Epic",
		"EpicGamesLauncher",
		"Data",
		"Manifests");

	public static string? FindEpicRlDir()
	{
		Console.WriteLine($"{Log.Blue}[RlTracker.Core.Config.FindEpicRlDir()]{Log.Reset}");
		try
		{
			string? installDir = FindEpicRlDirFromManifests();

			if (installDir != null)
			{
				Console.WriteLine($"{Log.Green}Epic RL dir (from manifests): {Log.Yellow}{installDir}.{Log.Reset}");
				return installDir;
			}
			foreach (string epicRoot in GetEpicRootCandidates())
			{
				installDir = Path.Combine(epicRoot, EpicRlDirName);

				if (IsRlInstallDir(installDir))
				{
					Console.WriteLine($"{Log.Green}Epic RL dir (from classic paths): {Log.Yellow}{installDir}.{Log.Reset}");
					return installDir;
				}
			}
			Console.WriteLine($"{Log.Red}Epic RL dir not found.{Log.Reset}");
			return null;
		}
		catch (Exception exception)
		{
			Console.WriteLine($"{Log.Red}Exception: {exception.Message}.{Log.Reset}");
			return null;
		}
	}

	private static string? FindEpicRlDirFromManifests()
	{
		if (!Directory.Exists(EpicManifestsDir))
			return null;
		try
		{
			foreach (string manifestFile in Directory.EnumerateFiles(EpicManifestsDir, EpicManifestPattern))
			{
				if (TryGetManifestInstallLocation(manifestFile, out string installDir)
					&& IsRlInstallDir(installDir))
					return installDir;
			}
		}
		catch
		{
			return null;
		}
		return null;
	}

	private static bool TryGetManifestInstallLocation(string manifestFile, out string installDir)
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

	private static IEnumerable<string> GetEpicRootCandidates()
	{
		if (!string.IsNullOrWhiteSpace(ProgramFiles))
			yield return Path.Combine(ProgramFiles, EpicGamesDirName);
		if (!string.IsNullOrWhiteSpace(ProgramFilesX86))
			yield return Path.Combine(ProgramFilesX86, EpicGamesDirName);
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
